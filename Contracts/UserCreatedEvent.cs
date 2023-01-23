using System;
using MessageBus.Interfaces;

namespace Contracts;

public sealed record UserCreatedEvent(string Name, string Email) : IEvent
{
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}