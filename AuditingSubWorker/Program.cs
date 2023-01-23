using AuditingSubWorker;
using AuditingSubWorker.Handlers;
using Contracts;
using MessageBus;
using MessageBus.Interfaces;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.RegisterRabbitMqEventConsumer(configuration.GetSection("RabbitMQ").Bind);
        services.AddScoped<AuditingEventHandler>();
    })
    .Build();

var services = host.Services;

var consumer = services.GetRequiredService<IEventConsumer>();
consumer.Subscribe<UserCreatedEvent, AuditingEventHandler>();

host.Run();