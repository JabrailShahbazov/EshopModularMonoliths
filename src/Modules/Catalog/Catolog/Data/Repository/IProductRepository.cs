using Shared.Data.Repository;

namespace Catalog.Data.Repository;

/// <summary>
/// Product-specific repository interface.
/// Extends generic repository with product-specific operations.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Gets products by category with pagination.
    /// </summary>
    Task<(IEnumerable<Product> Products, int TotalCount)> GetProductsByCategoryAsync(
        string category,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by name filter with pagination.
    /// </summary>
    Task<(IEnumerable<Product> Products, int TotalCount)> GetProductsByNameAsync(
        string name,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated products with filtering.
    /// </summary>
    Task<(IEnumerable<Product> Products, int TotalCount)> GetProductsAsync(
        int pageIndex,
        int pageSize,
        string? categoryFilter = null,
        string? nameFilter = null,
        CancellationToken cancellationToken = default);
}

