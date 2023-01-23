namespace MessageBus.Interfaces
{
    public interface IEventPublisher
    {
        void PublishEvent(IEvent @event);
    }
}