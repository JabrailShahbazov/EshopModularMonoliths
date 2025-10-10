using Shared.DDD.Domain.Entities;
using Shared.DDD.Domain.Events;

namespace Shared.DDD.Domain.Aggregate;

/// <summary>
/// Represents a generic aggregate root in the domain.
/// </summary>
/// <typeparam name="T">Type of the aggregate identifier.</typeparam>
public interface IAggregate<T> : IAggregate, IEntity<T>
{
}

/// <summary>
/// Represents an aggregate root with domain event support.
/// </summary>
public interface IAggregate : IEntity
{
    /// <summary>
    /// Gets the list of domain events associated with the aggregate.
    /// </summary>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all domain events from the aggregate and returns them.
    /// </summary>
    IDomainEvent[] ClearDomainEvents();
}