using Shared.Data.UnitOfWork;

namespace Ordering.Data.Repository;

public interface IOrderingUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Gets the Order repository.
    /// </summary>
    IOrderRepository Orders { get; }
}