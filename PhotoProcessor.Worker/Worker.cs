 // PhotoProcessor.Worker/Worker.cs
   using SixLabors.ImageSharp;
   using SixLabors.ImageSharp.Processing;
   using PhotoProcessor.Api.Data; // Para acessar o ApplicationDbContext
   using PhotoProcessor.Api.models; // Para acessar o modelo Image
   using Microsoft.EntityFrameworkCore; // Para usar métodos de extensão do EF Core
   using System;
   using System.IO;
   using System.Threading;
   using System.Threading.Tasks;
   using Microsoft.Extensions.DependencyInjection; // Para IServiceProvider e CreateScope
   using Microsoft.Extensions.Logging; // Para ILogger

   // Adiciona um alias para a classe Image da biblioteca SixLabors.ImageSharp
   // Isso resolve a ambiguidade com a sua classe PhotoProcessor.Api.Models.Image
   using SixLaborsImage = SixLabors.ImageSharp.Image;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider; // Para criar escopos de DbContext

    // Caminhos das pastas - ajustados para funcionar dentro do contêiner
    private const string UploadsFolder = "uploads";
    private const string OriginalsFolder = "originals";
    private const string ProcessedFolder = "processed";

    private readonly string _uploadsPath = Path.Combine(UploadsFolder);
    private readonly string _processedPath = Path.Combine(ProcessedFolder);
    private readonly string _originalsPath = Path.Combine(OriginalsFolder);

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider; // Injetamos o ServiceProvider para criar escopos
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado em: {time}", DateTimeOffset.Now);

        // Garante que as pastas de destino existam
        Directory.CreateDirectory(_processedPath);
        Directory.CreateDirectory(_originalsPath);

        // Este loop roda para sempre, até que a aplicação seja parada
        while (!stoppingToken.IsCancellationRequested)
        {
            // Criamos um escopo para o DbContext a cada iteração do loop.
            // Isso é uma boa prática para serviços de longa duração,
            // pois garante que o DbContext não fique muito tempo aberto
            // e que recursos sejam liberados.
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // 1. Busca imagens com status "Pending" no banco de dados
                var pendingImages = await dbContext.Images
                                                    .Where(i => i.Status == "Pending")
                                                    .ToListAsync(stoppingToken);

                if (pendingImages.Any()) // Adicionado para logar se encontrou algo
                {
                    _logger.LogInformation("Encontradas {count} imagens pendentes para processar.", pendingImages.Count);
                }
                else
                {
                    _logger.LogDebug("Nenhuma imagem pendente encontrada no momento."); // Use LogDebug para não poluir demais
                }


                foreach (var imageRecord in pendingImages)
                {
                    var filePath = Path.Combine(_uploadsPath, imageRecord.OriginalFileName);

                    // Verifica se o arquivo realmente existe na pasta de uploads local
                    if (!File.Exists(filePath))
                    {
                        _logger.LogWarning("Arquivo {fileName} (ID: {imageId}) não encontrado na pasta de uploads local. Marcando como falha.",
                            imageRecord.OriginalFileName, imageRecord.Id);
                        imageRecord.Status = "Failed";
                        imageRecord.UpdatedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync(stoppingToken);
                        continue; // Pula para a próxima imagem
                    }

                    try
                    {
                        _logger.LogInformation("Processando imagem do DB: {fileName} (ID: {imageId})",
                            imageRecord.OriginalFileName, imageRecord.Id);

                        // 2. Atualiza o status no banco para "Processing"
                        imageRecord.Status = "Processing";
                        imageRecord.UpdatedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync(stoppingToken);

                        // Carrega a imagem usando o alias SixLaborsImage
                        using (var sixLaborsImage = await SixLaborsImage.LoadAsync(filePath, stoppingToken)) // <-- MUDANÇA AQUI
                        {
                            // Cria a miniatura (thumbnail)
                            var newSize = new ResizeOptions
                            {
                                Size = new Size(200, 0),
                                Mode = ResizeMode.Max
                            };
                            sixLaborsImage.Mutate(x => x.Resize(newSize)); // <-- MUDANÇA AQUI

                            // Salva a nova imagem na pasta de processados
                            var processedFileName = $"thumbnail_{imageRecord.OriginalFileName}";
                            var processedFilePath = Path.Combine(_processedPath, processedFileName);
                            await sixLaborsImage.SaveAsync(processedFilePath, stoppingToken); // <-- MUDANÇA AQUI
                            _logger.LogInformation("Thumbnail salvo em: {path}", processedFilePath);

                            // 3. Atualiza o registro no banco de dados com o status "Completed"
                            // e o link para a imagem processada (ainda local por enquanto)
                            imageRecord.Status = "Completed";
                            imageRecord.ProcessedFileUrl = processedFilePath; // Usando o caminho local por enquanto
                            imageRecord.UpdatedAt = DateTime.UtcNow;
                            await dbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation("Processamento da imagem {fileName} (ID: {imageId}) concluído e status atualizado no DB.",
                                imageRecord.OriginalFileName, imageRecord.Id);
                        }

                        // Move o arquivo original para a pasta de originais após o processamento
                        var originalDestinationPath = Path.Combine(_originalsPath, imageRecord.OriginalFileName);
                        File.Move(filePath, originalDestinationPath);
                        _logger.LogInformation("Arquivo original {fileName} movido para: {path}",
                            imageRecord.OriginalFileName, originalDestinationPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar a imagem {fileName} (ID: {imageId}). Marcando como falha.",
                            imageRecord.OriginalFileName, imageRecord.Id);
                        // Em caso de erro, atualiza o status para "Failed"
                        imageRecord.Status = "Failed";
                        imageRecord.UpdatedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync(stoppingToken);
                        // Opcional: mover o arquivo para uma pasta de erros
                    }
                }
            } // O escopo do DbContext é encerrado aqui

            // Espera 5 segundos antes de verificar novamente
            await Task.Delay(5000, stoppingToken);
        }
    }
}