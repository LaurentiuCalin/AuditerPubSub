namespace MessageBus.Interfaces;

public interface IEventPublisher
{
    /// <summary>
    /// Publish event
    /// </summary>
    /// <param name="event">Event to be submitted to registered queue</param>
    void PublishEvent(IEvent @event);
}