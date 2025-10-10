using Shared.DDD.Domain.Entities;
using Shared.DDD.Domain.Events;

namespace Shared.DDD.Domain.Aggregate;

/// <summary>
/// Represents an aggregate root in the domain-driven design pattern.
/// Aggregates encapsulate domain entities and manage domain events.
/// </summary>
/// <typeparam name="TId">Type of the aggregate identifier.</typeparam>
public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
{
    /// <summary>
    /// Internal list of domain events raised by the aggregate.
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the read-only list of domain events.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the aggregate.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events and returns them.
    /// </summary>
    /// <returns>An array of cleared domain events.</returns>
    public IDomainEvent[] ClearDomainEvents()
    {
        var events = _domainEvents.ToArray();
        _domainEvents.Clear();
        return events;
    }
}