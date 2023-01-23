using Contracts;
using MessageBus.Interfaces;
using Newtonsoft.Json;

namespace AuditingSubWorker.Handlers;

public sealed class AuditingEventHandler : IEventHandler<UserCreatedEvent>
{
    private readonly ILogger<AuditingEventHandler> _logger;

    public AuditingEventHandler(ILogger<AuditingEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(UserCreatedEvent userCreatedEvent)
    {
        var auditLog = $"New user was created at {userCreatedEvent.OccuredAt}. User is: {JsonConvert.SerializeObject(userCreatedEvent)}";
        _logger.LogInformation(auditLog);

        return Task.CompletedTask;
    }
}