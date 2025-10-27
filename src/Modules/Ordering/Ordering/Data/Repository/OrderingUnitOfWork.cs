using Shared.Data.UnitOfWork;

namespace Ordering.Data.Repository;

/// <summary>
/// Ordering Unit of Work implementation.
/// Coordinates work across multiple repositories in the Ordering context.
/// </summary>
public class OrderingUnitOfWork : UnitOfWork<OrderingDbContext>, IOrderingUnitOfWork
{
    private readonly OrderingDbContext _context;
    private IOrderRepository? _orderRepository;

    public OrderingUnitOfWork(OrderingDbContext context) : base(context)
    {
        _context = context;
    }

    public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);
}


