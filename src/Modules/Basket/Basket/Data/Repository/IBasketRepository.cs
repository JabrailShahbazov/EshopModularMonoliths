﻿using Shared.Data.Repository;

namespace Basket.Data.Repository;

/// <summary>
/// Basket-specific repository interface.
/// Extends generic repository with basket-specific operations.
/// </summary>
public interface IBasketRepository : IRepository<ShoppingCart>
{
    Task<ShoppingCart?> GetBasketByUserNameAsync(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default);

    Task<bool> DeleteBasketByUserNameAsync(string userName, CancellationToken cancellationToken = default);
}