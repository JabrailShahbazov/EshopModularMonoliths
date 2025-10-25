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
    
    public async Task<ShoppingCart> GetBasketAsync(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        if (!asNoTracking)
        {
            return await basketRepository.GetBasketAsync(userName, false, cancellationToken);
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

        var basket = await basketRepository.GetBasketAsync(userName, asNoTracking, cancellationToken);

        await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket, _options), cancellationToken);

        return basket;
    }

    public async Task<ShoppingCart> CreateBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        await basketRepository.CreateBasketAsync(basket, cancellationToken);

        await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket, _options), cancellationToken);

        return basket;
    }

    public async Task<bool> DeleteBasketAsync(string userName, CancellationToken cancellationToken = default)
    {
        var result = await basketRepository.DeleteBasketAsync(userName, cancellationToken);

        if (result)
        {
            await cache.RemoveAsync(userName, cancellationToken);
        }

        return result;
    }

    public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
    {
        var result = await basketRepository.SaveChangesAsync(userName, cancellationToken);
        
        if (userName is not null)
        {
            await cache.RemoveAsync(userName, cancellationToken);
        }

        return result;
    }
}