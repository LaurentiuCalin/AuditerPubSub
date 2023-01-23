using System;
using MessageBus.Interfaces;
using MessageBus.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;

namespace MessageBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterRabbitMqEventPublisher(this IServiceCollection services, Action<RabbitMqOptions> rabbitMqConfiguration)
        {
            var rabbitMqOptions = new RabbitMqOptions();
            rabbitMqConfiguration(rabbitMqOptions);
            var factory = GetFactory(rabbitMqOptions);

            services.AddSingleton<IEventPublisher>(provider =>
            {
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();

                return new RabbitMqEventPublisher(
                    connection,
                    channel,
                    provider.GetLogger<RabbitMqEventPublisher>(),
                    rabbitMqOptions.QueueName);
            });

            return services;
        }

        public static IServiceCollection RegisterRabbitMqEventConsumer(this IServiceCollection services, Action<RabbitMqOptions> rabbitMqConfiguration)
        {
            var rabbitMqOptions = new RabbitMqOptions();
            rabbitMqConfiguration(rabbitMqOptions);
            var factory = GetFactory(rabbitMqOptions);

            services.AddSingleton<IEventConsumer>(provider =>
            {
                var scopedServiceFactory = provider.GetRequiredService<IServiceScopeFactory>();
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();

                return new RabbitMqEventConsumer(
                    rabbitMqOptions.QueueName,
                    channel,
                    provider.GetLogger<RabbitMqEventConsumer>(),
                    scopedServiceFactory
                );
            });

            return services;
        }

        private static ConnectionFactory GetFactory(RabbitMqOptions options)
        {
            return new ConnectionFactory
            {
                HostName = options.Hostname,
                Port = options.Port,
                UserName = options.Username,
                Password = options.Password,
                DispatchConsumersAsync = true,
            };
        }

        private static ILogger<T> GetLogger<T>(this IServiceProvider provider)
        {
            var loggerFactory = provider.GetService<ILoggerFactory>();
            return loggerFactory != null
                ? loggerFactory.CreateLogger<T>()
                : NullLogger<T>.Instance;
        }
    }
}