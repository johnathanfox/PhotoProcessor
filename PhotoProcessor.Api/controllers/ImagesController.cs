    // PhotoProcessor.Api/Controllers/ImagesController.cs
    using Microsoft.AspNetCore.Mvc;
    using PhotoProcessor.Api.Data; // Para acessar o ApplicationDbContext
    using PhotoProcessor.Api.models; // Para acessar o modelo Image
    using System;
    using System.IO;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/images")]
    public class ImagesController : ControllerBase
    {
        // Injeto o ApplicationDbContext no meu controller.
        // O ASP.NET Core vai fornecer uma instância dele automaticamente.
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ImagesController> _logger; // Adicionando logger para mais detalhes

        public ImagesController(ApplicationDbContext dbContext, ILogger<ImagesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Nenhum arquivo enviado ou arquivo vazio.");
                return BadRequest("Nenhum arquivo enviado.");
            }

            // POR ENQUANTO: Salvar o arquivo localmente para testar
            // No futuro, isso será substituído pelo upload para o S3.
            var tempPath = Path.Combine("uploads");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            var filePath = Path.Combine(tempPath, file.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream); // Usar CopyToAsync para melhor performance
                }
                _logger.LogInformation("Arquivo {fileName} salvo localmente em {filePath}", file.FileName, filePath);

                // Crio um novo registro de imagem para o banco de dados
                var newImageRecord = new Image
                {
                    OriginalFileName = file.FileName,
                    OriginalFileUrl = filePath, // Por enquanto, o URL é o caminho local
                    Status = "Pending" // Definido como "Pending" para o Worker processar
                    // Id, CreatedAt e UpdatedAt são gerados automaticamente no construtor de Image
                };

                // Adiciono o registro ao DbContext e salvo no banco de dados
                _dbContext.Images.Add(newImageRecord);
                await _dbContext.SaveChangesAsync(); // Salva as mudanças no DB
                _logger.LogInformation("Registro da imagem {fileName} (ID: {imageId}) salvo no banco de dados com status 'Pending'.",
                    newImageRecord.OriginalFileName, newImageRecord.Id);

                // Futuramente, aqui enviaremos uma mensagem para uma fila (SQS)
                // Por enquanto, retornamos um OK.
                var response = new
                {
                    File = newImageRecord.OriginalFileName,
                    Id = newImageRecord.Id, // Incluo o ID para rastreamento
                    Size = file.Length,
                    Status = newImageRecord.Status,
                    Message = "Upload recebido e processamento agendado."
                };

                // Retorno um HTTP 202 Accepted para indicar que o processamento é assíncrono
                return Accepted(response); // Use Accepted para indicar processamento assíncrono
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar upload do arquivo {fileName}", file.FileName);
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }