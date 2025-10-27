using Basket.Data.Repository;
using Shared.Data.Extensions;

namespace Basket;

public static class BasketServiceCollectionExtensions
{
    public static IServiceCollection AddBasketServices(this IServiceCollection services)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddModuleRepositoryPattern<BasketDbContext, IBasketUnitOfWork, BasketUnitOfWork>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        
        // Decorate repository with caching
        services.Decorate<IBasketRepository, CachedBasketRepository>();
        return services;
    }
}