namespace shared.messaging.Events;

public record IntegrationEvent
{
    public Guid EventId => Guid.NewGuid();
    public DateTime OccuredOn => DateTime.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName!;
}

public record TaskAssignedIntegrationEvent : IntegrationEvent
{
    public Guid TaskId { get; init; }
    public Guid ProjectId { get; init; }
    public Guid AssignedUserId { get; init; }
    public DateTime AssignedAt { get; init; }
}