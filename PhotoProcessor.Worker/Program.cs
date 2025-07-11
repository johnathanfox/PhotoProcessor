// PhotoProcessor.Worker/Program.cs

// Este é o ponto de entrada do meu Worker Service.

// Primeiro, eu crio o 'host' da aplicação.
// Ele é responsável por gerenciar o ciclo de vida do meu serviço,
// carregando configurações e variáveis de ambiente, assim como na API.
using PhotoProcessor.Worker; // Importo o namespace do meu Worker para que ele seja encontrado

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Aqui eu registro o meu serviço de background (a classe Worker).
        // Isso informa ao .NET que minha classe 'Worker' deve ser executada
        // como um serviço de longa duração em segundo plano.
        services.AddHostedService<Worker>();
    })
    .Build();

// Finalmente, eu inicio o host, o que faz com que meu Worker comece a rodar.
host.Run();
