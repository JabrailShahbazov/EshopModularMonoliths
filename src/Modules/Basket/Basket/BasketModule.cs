using Basket.Data.Processors;
using Basket.Data.Repository;
using Shared.Data;
using Shared.Data.Extensions;

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

        // Register Repository and UnitOfWork using extension method
   

        services.AddHostedService<OutboxProcessor>();
        
        return services;
    }
    
    public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
    {
        app.UseMigration<BasketDbContext>();
        
        return app;
    }
}