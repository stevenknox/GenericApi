using System;
using Microsoft.EntityFrameworkCore;

namespace GenericApi
{
    public static class EntityFrameworkExtensions
    {
        public static void AddOrUpdate(this DbContext ctx, object entity)
        {
            var entry = ctx.Entry(entity);

            switch (entry.State)
            {
                case Microsoft.EntityFrameworkCore.EntityState.Detached:
                    ctx.Add(entity);
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Modified:
                    ctx.Update(entity);
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Added:
                    ctx.Add(entity);
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Unchanged:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void AddOrUpdate<T, Tid>(this DbContext ctx, T entity) where T : GenericEntity
        {
            var entry = ctx.Entry(entity);
            var stateInfo = entry.Entity;
            entry.State = GetEntityState(stateInfo.EntityState);

            switch (entry.State)
            {
                case Microsoft.EntityFrameworkCore.EntityState.Detached:
                    ctx.Add(entity);
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Modified:
                    ctx.Update(entity);
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Added:
                    ctx.Add(entity);
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Unchanged:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void ApplyStateChanges(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries<IEntityWithState>())
            {
                IEntityWithState stateInfo = entry.Entity;
                entry.State = GetEntityState(stateInfo.EntityState);
            }
        }

        public static Microsoft.EntityFrameworkCore.EntityState GetEntityState(EntityState entityState)
        {
            switch (entityState)
            {
                case EntityState.Unchanged:
                    return Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                case EntityState.Added:
                    return Microsoft.EntityFrameworkCore.EntityState.Added;
                case EntityState.Modified:
                    return Microsoft.EntityFrameworkCore.EntityState.Modified;
                case EntityState.Deleted:
                    return Microsoft.EntityFrameworkCore.EntityState.Deleted;
                default:
                    return Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }

    }
}
