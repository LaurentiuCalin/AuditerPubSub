using System;
using System.Text;
using MessageBus.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MessageBus.RabbitMq
{
    

    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqEventPublisher> _logger;
        private readonly string QueueName;

        public RabbitMqEventPublisher(
            IConnection connection,
            IModel channel,
            ILogger<RabbitMqEventPublisher> logger,
            string queueName)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        public void PublishEvent(IEvent @event)
        {
            try
            {
                var properties = _channel.CreateBasicProperties();
            
                // Can be made more configurable with custom attributes to avoid enforcing the same class name for pub/sub
                properties.Type = @event.GetType().Name; 

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

                _logger.LogInformation($"Publishing message {@event.GetType().Name}.");

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: QueueName,
                    mandatory: false,
                    properties,
                    body: body);
            }
            catch (Exception e)
            {
                _logger.LogError($"Message publishing failed. Message name {@event.GetType().Name}. Error message: {e.Message}.");
                throw;
            }
        }
    }
}