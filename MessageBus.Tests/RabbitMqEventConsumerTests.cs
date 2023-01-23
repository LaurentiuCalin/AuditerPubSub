using MessageBus.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace MessageBus.Tests;

public class RabbitMqEventConsumerTests
{
    private const string QueueName = "queue-name";
    private readonly Mock<IModel> _mockChannel = new(MockBehavior.Loose);
    private readonly Mock<ILogger<RabbitMqEventConsumer>> _logger = new();
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactory = new();
    
    [Fact]
    public void RabbitMqEventConsumer_CallsBasicConsume_WhenInstantiated()
    {
        // Arrange && Act
        var sut = new RabbitMqEventConsumer(QueueName, _mockChannel.Object, _logger.Object, _serviceScopeFactory.Object);

        // Assert
        _mockChannel.Verify(x => x.BasicConsume(QueueName,
                false,
                "",
                false,
                false,
                null,
                It.IsAny<AsyncEventingBasicConsumer>()),
            Times.Once);
    }
}