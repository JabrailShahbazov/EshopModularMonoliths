using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.Repository;
using Shared.Data.UnitOfWork;
using Shared.DDD.Domain.Entities;

namespace Shared.Data.Extensions;

/// <summary>
/// Extension methods for registering Repository and UnitOfWork patterns.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Registers generic repository and unit of work for a specific DbContext.
    /// </summary>
    /// <typeparam name="TContext">DbContext type</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddRepositoryPattern<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        // Register generic repository - note: this requires manual registration per entity type
        // services.AddScoped(typeof(IRepository<>), typeof(Repository<,>));
        
        // Register generic unit of work
        services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
        
        return services;
    }

    /// <summary>
    /// Registers both generic and specific repositories/unit of work for a module.
    /// </summary>
    /// <typeparam name="TContext">DbContext type</typeparam>
    /// <typeparam name="TUnitOfWork">Specific UnitOfWork interface</typeparam>
    /// <typeparam name="TUnitOfWorkImpl">Specific UnitOfWork implementation</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddModuleRepositoryPattern<TContext, TUnitOfWork, TUnitOfWorkImpl>(
        this IServiceCollection services)
        where TContext : DbContext
        where TUnitOfWork : class, IUnitOfWork
        where TUnitOfWorkImpl : class, TUnitOfWork
    {
        // Register generic pattern
        services.AddRepositoryPattern<TContext>();
        
        // Register module-specific unit of work
        services.AddScoped<TUnitOfWork, TUnitOfWorkImpl>();
        
        return services;
    }
}
