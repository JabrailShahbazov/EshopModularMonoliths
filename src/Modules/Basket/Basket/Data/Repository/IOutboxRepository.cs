using Shared.Data.Repository;

namespace Basket.Data.Repository;

public interface IOutboxRepository : IRepository<OutboxMessage>
{
}
