using Basket.Data.Repository;

namespace Basket;

public static class BasketServiceCollectionExtensions
{
    public static IServiceCollection AddBasketServices(this IServiceCollection services)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.Decorate<IBasketRepository, CachedBasketRepository>();
        
        return services;
    }
}