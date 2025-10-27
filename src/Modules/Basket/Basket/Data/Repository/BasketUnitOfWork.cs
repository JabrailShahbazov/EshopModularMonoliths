using Shared.Data.UnitOfWork;

namespace Basket.Data.Repository;

/// <summary>
/// Basket Unit of Work implementation.
/// Coordinates work across multiple repositories in the Basket context.
/// </summary>
public class BasketUnitOfWork : UnitOfWork<BasketDbContext>, IBasketUnitOfWork
{
    private readonly BasketDbContext _context;
    private IBasketRepository? _basketRepository;
    private IOutboxRepository? _outboxRepository;

    public BasketUnitOfWork(BasketDbContext context) : base(context)
    {
        _context = context;
    }

    public IBasketRepository Baskets => _basketRepository ??= new BasketRepository(_context);
    public IOutboxRepository Outbox => _outboxRepository ??= new OutboxRepository(_context);
}
