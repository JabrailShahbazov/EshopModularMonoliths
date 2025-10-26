using Basket.Data.Processors;
using Shared.Data;

namespace Basket;

public static class BasketModule
{
    public static IServiceCollection AddBasketModule(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddBasketServices();
        
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<BasketDbContext>((sr,options) =>
        {
            options.AddInterceptors(sr.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddHostedService<OutboxProcessor>();
        
        return services;
    }
    
    public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
    {
        app.UseMigration<BasketDbContext>();
        
        return app;
    }
}