using System;

namespace MessageBus.Interfaces
{
    public interface IEvent
    {
        public DateTime OccuredAt { get; }
    }
}