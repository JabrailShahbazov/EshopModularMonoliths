using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, 
                                                      IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.AddInterceptors(new AuditableEntityInterceptor());
            options.UseNpgsql(connectionString);
        });
        
        services.AddScoped<IDataSeeder, CatalogDataSeeder>();
        
        return services;
    }
    
    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
    {
        app.UseMigration<CatalogDbContext>();
        
        return app;
    }
}