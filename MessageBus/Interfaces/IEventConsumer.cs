namespace MessageBus.Interfaces;

public interface IEventConsumer
{
    void Subscribe<TEvent, TEventHandler>()
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>;
}