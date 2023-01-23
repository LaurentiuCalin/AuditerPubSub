using System.Collections.Concurrent;
using System.Text;
using MessageBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageBus.RabbitMq;

public class RabbitMqEventConsumer : IEventConsumer
{
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqEventConsumer> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    // We can further abstract this into ISubscriptionManager and provide different implementations
    // Like InMemorySubscriptionManager (pictured below) or say.. RedisSubscriptionManager
    private readonly ConcurrentDictionary<string, ISet<Type>> _availableHandlers = new();
    private readonly ConcurrentDictionary<string, Type> _availableEventTypes = new();

    public RabbitMqEventConsumer(
        string queueName,
        IModel model,
        ILogger<RabbitMqEventConsumer> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _channel = model ?? throw new ArgumentNullException(nameof(model));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

        EnsureCreated(_channel, queueName);
        StartListener(queueName);
    }

    internal void StartListener(string queueName)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnReceived;
        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }

    internal static void EnsureCreated(IModel channel, string queueName)
    {
        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    internal async Task OnReceived(object sender, BasicDeliverEventArgs @event)
    {
        _logger.LogInformation("Message received.");
        var message = Encoding.UTF8.GetString(@event.Body.ToArray());

        try
        {
            if (!_availableEventTypes.TryGetValue(@event.BasicProperties.Type, out var eventType))
            {
                _logger.LogWarning("No event types were found for the specified event");
                return;
            }

            var deserializeEvent = JsonConvert.DeserializeObject(message, eventType);

            if (!_availableHandlers.TryGetValue(@event.BasicProperties.Type, out var handlerTypes))
            {
                _logger.LogWarning("No handlers were found for the specified event");
                return;
            }

            await using var serviceScope = _serviceScopeFactory.CreateAsyncScope();

            foreach (var handlerType in handlerTypes)
            {
                var handler = serviceScope.ServiceProvider.GetRequiredService(handlerType);
                
                var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                // broadcast the message to subscribed handlers
                await Task.Yield();  // force async, non-blocking
                await ((Task)concreteType.GetMethod("Handle")!.Invoke(handler, new[] { deserializeEvent })!);
            }

            _channel.BasicAck(@event.DeliveryTag, false);
        }
        catch (Exception e)
        {
            _logger.LogError($"Message handling failed. Message name {@event.BasicProperties.Type}. Error message: {e.Message}.");
            _channel.BasicReject(@event.DeliveryTag, true);
        }
    }

    public void Subscribe<TEvent, TEventHandler>()
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;

        _availableEventTypes.TryAdd(eventName, typeof(TEvent));
        // if false, entry already exists

        var handlers = _availableHandlers.GetOrAdd(eventName, _ => new HashSet<Type>());
        handlers.Add(typeof(TEventHandler));
        _availableHandlers[eventName] = handlers;
    }
}