// Início da aplicação (ponto de entrada)
using PhotoProcessor.Api.Data; // Importa o namespace do contexto de dados
using Microsoft.EntityFrameworkCore; // Importa o namespace do Entity Framework Core

var builder = WebApplication.CreateBuilder(args);

// Registro os serviços que vou usar na API

// Adiciono os controllers da aplicação
builder.Services.AddControllers();

// Swagger pra gerar a doc e UI de teste
builder.Services.AddEndpointsApiExplorer();
 builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSwaggerGen();

// Agora eu construo a aplicação
var app = builder.Build();

// Aplicar migrações automaticamente
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações do banco de dados.");
    }
}

// Configuro o pipeline de requisições

// Deixo o Swagger ativo só no ambiente de dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Forço o redirecionamento pra HTTPS
app.UseHttpsRedirection();

// Ativo o sistema de autorização (ainda sem usar, mas já deixo)
app.UseAuthorization();

// Mapeio os endpoints pros controllers
app.MapControllers();

// Starto o servidor
app.Run();
