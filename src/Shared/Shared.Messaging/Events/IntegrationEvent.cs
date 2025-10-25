namespace Shared.Messaging.Events;

public record IntegrationEvent
{
    public Guid EventId => Guid.NewGuid();
    
    public DateTimeOffset OccurredOn => DateTime.Now;
    
    public string? EventType => GetType().AssemblyQualifiedName;
}