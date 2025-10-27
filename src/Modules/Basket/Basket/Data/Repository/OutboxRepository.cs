using Shared.Data.Repository;

namespace Basket.Data.Repository;

public class OutboxRepository : Repository<OutboxMessage, BasketDbContext>, IOutboxRepository
{
    public OutboxRepository(BasketDbContext context) : base(context)
    {
    }
}
