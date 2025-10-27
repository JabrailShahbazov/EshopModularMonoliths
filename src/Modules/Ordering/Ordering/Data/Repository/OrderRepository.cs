using Microsoft.EntityFrameworkCore;
using Shared.Data.Repository;

namespace Ordering.Data.Repository;

/// <summary>
/// Order repository implementation using EF Core.
/// </summary>
public class OrderRepository : Repository<Order, OrderingDbContext>, IOrderRepository
{
    public OrderRepository(OrderingDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetOrdersAsync(
        int pageIndex,
        int pageSize,
        string? orderNameFilter = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Include(o => o.Items).AsQueryable();

        if (!string.IsNullOrWhiteSpace(orderNameFilter))
        {
            query = query.Where(o => o.OrderName.Contains(orderNameFilter));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (orders, totalCount);
    }
}
