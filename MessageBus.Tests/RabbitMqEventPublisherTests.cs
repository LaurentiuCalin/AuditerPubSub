using System.Text;
using AutoFixture;
using MessageBus.Interfaces;
using MessageBus.RabbitMq;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Xunit;

namespace MessageBus.Tests;

public class RabbitMqEventPublisherTests
{
    private readonly Mock<IConnection> _mockConnection = new();
    private readonly Mock<IModel> _mockChannel = new(MockBehavior.Loose);
    private readonly Mock<ILogger<RabbitMqEventPublisher>> _logger = new();
    private readonly RabbitMqEventPublisher _sut;
    private readonly IFixture _fixture = new Fixture();
    private const string _queueName = "queue-name";

    public RabbitMqEventPublisherTests()
    {
        _sut = new RabbitMqEventPublisher(_mockConnection.Object, _mockChannel.Object, _logger.Object, _queueName);
    }

    [Fact]
    public void Publish_ShouldPublishMessage_WhenExecuted()
    {
        // Arrange
        var message = _fixture.Create<TestableMessage>();
        var basicProps = Mock.Of<IBasicProperties>();

        _mockChannel
            .Setup(x => x.CreateBasicProperties())
            .Returns(basicProps);

        TestableMessage messageToAssert = default!;
        _mockChannel
            .Setup(x => x.BasicPublish("",
                _queueName,
                false,
                basicProps,
                It.IsAny<ReadOnlyMemory<byte>>()))
            .Callback((
                string _,
                string _,
                bool _,
                IBasicProperties _,
                ReadOnlyMemory<byte> body) =>
            {
                messageToAssert = JsonConvert.DeserializeObject<TestableMessage>(Encoding.UTF8.GetString(body.ToArray()))!;
            });

        // Act
        _sut.PublishEvent(message);

        // Assert
        _mockChannel.Verify(x => x.BasicPublish(
            "",
            _queueName,
            false,
            basicProps,
            It.IsAny<ReadOnlyMemory<byte>>()
        ), Times.Once);

        Assert.Equal(message, messageToAssert);
    }

    public sealed record TestableMessage(long Id, string message) : IEvent
    {
        public DateTime OccuredAt { get; } = new DateTime(2023, 01, 01, 1, 1, 1, 1);
    }
}