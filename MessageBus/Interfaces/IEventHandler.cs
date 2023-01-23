using System.Threading.Tasks;

namespace MessageBus.Interfaces
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task Handle(TEvent @event);
    }
}