using Shared.Data.UnitOfWork;

namespace Basket.Data.Repository;

/// <summary>
/// Basket Unit of Work implementation.
/// Coordinates work across multiple repositories in the Basket context.
/// </summary>
public class BasketUnitOfWork : UnitOfWork<BasketDbContext>, IBasketUnitOfWork
{
    public IBasketRepository Baskets { get; }
    public IOutboxRepository Outbox { get; }

    public BasketUnitOfWork(
        BasketDbContext context,
        IBasketRepository baskets,
        IOutboxRepository outbox) : base(context)
    {
        Baskets = baskets;
        Outbox = outbox;
    }
}
