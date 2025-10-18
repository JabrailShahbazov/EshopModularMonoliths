namespace Basket;

public static class BasketServiceCollectionExtensions
{
    public static IServiceCollection AddBasketServices(this IServiceCollection services)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        
        return services;
    }
}