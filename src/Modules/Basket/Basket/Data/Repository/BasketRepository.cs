﻿using Microsoft.EntityFrameworkCore;
using Shared.Data.Repository;

namespace Basket.Data.Repository;

public class BasketRepository : Repository<ShoppingCart, BasketDbContext>, IBasketRepository
{
    public BasketRepository(BasketDbContext context) : base(context)
    {
    }

    public async Task<ShoppingCart?> GetBasketByUserNameAsync(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Include(b => b.Items).Where(x => x.UserName == userName);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> DeleteBasketByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        var basket = await GetBasketByUserNameAsync(userName, false, cancellationToken);

        if (basket == null)
            return false;

        DbSet.Remove(basket);
        return true;
    }
}