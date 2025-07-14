// PhotoProcessor.Worker/Program.cs

// Este é o meu ponto de entrada para o Worker Service.

// Importo os namespaces que preciso para configurar o host e o banco de dados.
using PhotoProcessor.Api.Data; // Preciso acessar o ApplicationDbContext que está na API
using Microsoft.EntityFrameworkCore; // Para usar o Npgsql com o Entity Framework Core
using Microsoft.Extensions.Hosting; // Essencial para IHost e HostBuilderContext
using Microsoft.Extensions.DependencyInjection; // Para registrar serviços como AddHostedService e GetConnectionString
using Microsoft.Extensions.Configuration; // Para acessar as configurações, tipo a string de conexão

// Crio o 'host' da minha aplicação Worker.
// Ele é responsável por carregar as configurações (como do appsettings.json)
// e gerenciar o ciclo de vida do meu serviço em segundo plano.
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) => // Aqui eu configuro os serviços que meu Worker vai usar.
    {
        // Registro meu Worker principal como um serviço de background.
        // Isso faz com que o .NET execute a lógica que defini no Worker.cs.
        services.AddHostedService<Worker>();

        // Configuro a conexão com o banco de dados PostgreSQL.
        // Uso o ApplicationDbContext, que está no projeto da API,
        // e pego a string de conexão do meu arquivo de configurações (appsettings.json do Worker).
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));
    })
    .Build(); // Construo o host com todas as configurações e serviços definidos.

// Finalmente, inicio o host. Isso faz com que o meu Worker comece a rodar e a fazer seu trabalho.
host.Run();