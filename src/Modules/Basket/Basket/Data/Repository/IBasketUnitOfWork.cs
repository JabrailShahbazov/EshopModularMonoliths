using Shared.Data.UnitOfWork;

namespace Basket.Data.Repository;

/// <summary>
/// Basket module specific Unit of Work.
/// Provides access to all repositories in the Basket context.
/// </summary>
public interface IBasketUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Gets the Basket repository.
    /// </summary>
    IBasketRepository Baskets { get; }
    
    /// <summary>
    /// Gets the Outbox repository.
    /// </summary>
    IOutboxRepository Outbox { get; }
}
