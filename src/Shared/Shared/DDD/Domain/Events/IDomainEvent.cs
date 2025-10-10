using MediatR;

namespace Shared.DDD.Domain.Events;

/// <summary>
/// Represents a domain event in the Domain-Driven Design (DDD) pattern.
/// Inherits from <see cref="INotification"/> for MediatR integration.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Gets a unique identifier for the event instance.
    /// </summary>
    Guid EventId => Guid.NewGuid();

    /// <summary>
    /// Gets the UTC date and time when the event occurred.
    /// </summary>
    DateTime OccurredOn => DateTime.UtcNow;

    /// <summary>
    /// Gets the fully qualified type name of the event.
    /// </summary>
    string EventType => GetType().AssemblyQualifiedName!;
    
    /// <summary>
    /// Gets the identifier of the user or system that triggered the event.
    /// </summary>
    string? TriggeredBy => string.Empty;
}