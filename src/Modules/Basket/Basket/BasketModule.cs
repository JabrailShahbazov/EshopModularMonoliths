namespace Basket;

public static class BasketModule
{
    public static IServiceCollection AddBasketModule(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddBasketModule(configuration);
        
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<BasketDbContext>((sr,options) =>
        {
            options.AddInterceptors(sr.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });
        
        return services;
    }
    
    public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
    {
        return app;
    }
}