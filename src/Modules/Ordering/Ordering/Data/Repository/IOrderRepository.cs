using Shared.Data.Repository;

namespace Ordering.Data.Repository;

/// <summary>
/// Order-specific repository interface.
/// Extends generic repository with order-specific operations.
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Gets orders by customer ID with related items.
    /// </summary>
    Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order by ID with all related data (items, etc.).
    /// </summary>
    Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated orders with filtering.
    /// </summary>
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetOrdersAsync(
        int pageIndex,
        int pageSize,
        string? orderNameFilter = null,
        CancellationToken cancellationToken = default);
}

