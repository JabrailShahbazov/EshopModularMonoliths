using Shared.Data.UnitOfWork;

namespace Catalog.Data.Repository;

/// <summary>
/// Catalog module specific Unit of Work.
/// Provides access to all repositories in the Catalog context.
/// </summary>
public interface ICatalogUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Gets the Product repository.
    /// </summary>
    IProductRepository Products { get; }
}

