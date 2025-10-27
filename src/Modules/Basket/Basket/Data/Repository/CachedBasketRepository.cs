using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Basket.Data.JsonConverters;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Data.Repository;

public class CachedBasketRepository(IBasketRepository basketRepository, IDistributedCache cache) : IBasketRepository
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new ShoppingCartConverter(), new ShoppingCartItemConverter() }
    };
    
    public async Task<ShoppingCart?> GetBasketByUserNameAsync(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        if (!asNoTracking)
        {
            return await basketRepository.GetBasketByUserNameAsync(userName, false, cancellationToken);
        }

        var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);

        if (!string.IsNullOrEmpty(cachedBasket))
        {
            var deserialize = JsonSerializer.Deserialize<ShoppingCart>(cachedBasket, _options);

            if (deserialize is not null)
            {
                return deserialize;
            }
        }

        var basket = await basketRepository.GetBasketByUserNameAsync(userName, asNoTracking, cancellationToken);

        if (basket != null)
        {
            await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket, _options), cancellationToken);
        }

        return basket;
    }

    public async Task<bool> DeleteBasketByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        var result = await basketRepository.DeleteBasketByUserNameAsync(userName, cancellationToken);
        
        if (result)
        {
            await cache.RemoveAsync(userName, cancellationToken);
        }

        return result;
    }

    // Generic repository methods - delegate to underlying repository with cache management
    public async Task<ShoppingCart> AddAsync(ShoppingCart entity, CancellationToken cancellationToken = default)
    {
        var result = await basketRepository.AddAsync(entity, cancellationToken);
        await cache.SetStringAsync(entity.UserName, JsonSerializer.Serialize(result, _options), cancellationToken);
        return result;
    }

    public async Task AddRangeAsync(IEnumerable<ShoppingCart> entities, CancellationToken cancellationToken = default)
    {
        var list = entities.ToList();
        await basketRepository.AddRangeAsync(list, cancellationToken);
        foreach (var e in list)
        {
            await cache.SetStringAsync(e.UserName, JsonSerializer.Serialize(e, _options), cancellationToken);
        }
    }

    public void Update(ShoppingCart entity)
    {
        basketRepository.Update(entity);
        _ = cache.RemoveAsync(entity.UserName); // fire-and-forget invalidation
    }

    public void UpdateRange(IEnumerable<ShoppingCart> entities)
    {
        basketRepository.UpdateRange(entities);
        foreach (var e in entities)
        {
            _ = cache.RemoveAsync(e.UserName);
        }
    }

    public void Remove(ShoppingCart entity)
    {
        basketRepository.Remove(entity);
        _ = cache.RemoveAsync(entity.UserName);
    }

    public void RemoveRange(IEnumerable<ShoppingCart> entities)
    {
        basketRepository.RemoveRange(entities);
        foreach (var e in entities)
        {
            _ = cache.RemoveAsync(e.UserName);
        }
    }

    public Task<ShoppingCart?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
    {
        return basketRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IEnumerable<ShoppingCart>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return basketRepository.GetAllAsync(cancellationToken);
    }

    public Task<IEnumerable<ShoppingCart>> FindAsync(Expression<Func<ShoppingCart, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return basketRepository.FindAsync(predicate, cancellationToken);
    }

    public Task<ShoppingCart?> FirstOrDefaultAsync(Expression<Func<ShoppingCart, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return basketRepository.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<ShoppingCart?> SingleOrDefaultAsync(Expression<Func<ShoppingCart, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return basketRepository.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<ShoppingCart, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return basketRepository.AnyAsync(predicate, cancellationToken);
    }

    public Task<int> CountAsync(Expression<Func<ShoppingCart, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return basketRepository.CountAsync(predicate, cancellationToken);
    }

    public IQueryable<ShoppingCart> AsQueryable()
    {
        return basketRepository.AsQueryable();
    }
}
