// Início da aplicação (ponto de entrada)

// Crio o builder da aplicação, que carrega configs e variáveis de ambiente
var builder = WebApplication.CreateBuilder(args);

// Registro os serviços que vou usar na API

// Adiciono os controllers da aplicação
builder.Services.AddControllers();

// Swagger pra gerar a doc e UI de teste
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agora eu construo a aplicação
var app = builder.Build();

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
