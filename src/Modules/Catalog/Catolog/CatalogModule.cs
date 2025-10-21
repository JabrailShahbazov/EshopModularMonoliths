using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, 
                                                      IConfiguration configuration)
    {
        services.AddCatalogServices();
        
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<CatalogDbContext>((sr,options) =>
        {
            options.AddInterceptors(sr.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });
        
        return services;
    }
    
    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
    {
        app.UseMigration<CatalogDbContext>();
        
        return app;
    }
}