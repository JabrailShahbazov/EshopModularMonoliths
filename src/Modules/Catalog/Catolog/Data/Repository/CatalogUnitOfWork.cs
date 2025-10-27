using Shared.Data.UnitOfWork;

namespace Catalog.Data.Repository;

/// <summary>
/// Catalog Unit of Work implementation.
/// Coordinates work across multiple repositories in the Catalog context.
/// </summary>
public class CatalogUnitOfWork : UnitOfWork<CatalogDbContext>, ICatalogUnitOfWork
{
    private readonly CatalogDbContext _context;
    private IProductRepository? _productRepository;

    public CatalogUnitOfWork(CatalogDbContext context) : base(context)
    {
        _context = context;
    }

    public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
}
