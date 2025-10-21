namespace Basket.Data.Repository;

public class BasketRepository(BasketDbContext dbContext) : IBasketRepository
{
    public async Task<ShoppingCart> GetBasketAsync(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        var query = dbContext.ShoppingCarts.Include(b => b.Items).Where(x => x.UserName == userName);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        var basket = await query.SingleOrDefaultAsync(cancellationToken);

        return basket ?? throw new BasketNotFoundException(userName);
    }

    public async Task<ShoppingCart> CreateBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        dbContext.ShoppingCarts.Add(basket);

        await SaveChangesAsync(cancellationToken);

        return basket;
    }

    public async Task<bool> DeleteBasketAsync(string userName, CancellationToken cancellationToken = default)
    {
        var basket = await GetBasketAsync(userName, false, cancellationToken);

        dbContext.ShoppingCarts.Remove(basket);

        var result = await SaveChangesAsync(cancellationToken);

        return result > 0;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}