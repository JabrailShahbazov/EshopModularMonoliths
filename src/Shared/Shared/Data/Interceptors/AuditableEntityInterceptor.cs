using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.DDD.Domain.Entities;

namespace Shared.Data.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? eventDataContext)
    {
        if (eventDataContext == null) return;

        var entries = eventDataContext.ChangeTracker.Entries<IEntity>();

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = "Jabrail";
                entry.Entity.CreatedAt = now;
            }

            if (entry.HasChangeOwnedEntities())
            {
                entry.Entity.LastModifiedBy = "Jabrail";
                entry.Entity.LastModified = now;
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangeOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r => r.TargetEntry != null &&
                                  r.TargetEntry.Metadata.IsOwned() &&
                                  (r.TargetEntry.State == EntityState.Modified));
}