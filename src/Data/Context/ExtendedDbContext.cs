using Microsoft.EntityFrameworkCore;
using ORBIT9000.Abstractions.Data.Entities;
using System.Reflection;

namespace ORBIT9000.Data.Context
{
    public class ExtendedDbContext : DbContext
    {
        private static Type[]? _entities;
        private readonly bool _created;

        private static IEnumerable<Type> Entities
        {
            get
            {
                //HACK: for some reason entities are loaded twice.
                _entities ??= [..AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes()
                        .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(IEntity))))
                    .GroupBy(type => type.AssemblyQualifiedName)
                    .Select(group => group.First())
                ];

                return _entities;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Type extended = typeof(IExtendedEntity<Guid>);

            string[] propertyNames =
            [
                    nameof(IExtendedEntity.CreatedOn),
                    nameof(IExtendedEntity.CreatedBy),
                    nameof(IExtendedEntity.DeletedOn),
                    nameof(IExtendedEntity.DeletedBy),
                    nameof(IExtendedEntity.ModifiedOn),
                    nameof(IExtendedEntity.ModifiedBy)
            ];

            foreach (Type entity in Entities)
            {
                modelBuilder.Entity(entity).HasKey(nameof(IEntity.Id));

                foreach (string propName in propertyNames)
                {
                    PropertyInfo? propInfo = extended.GetProperty(propName);
                    if (propInfo != null)
                    {
                        modelBuilder.Entity(entity).Property(propInfo.PropertyType, propName);
                    }
                }
            }
        }

        public override int SaveChanges()
        {
            if (!_created)
            {
                Database.EnsureCreated();
            }

            ChangeTracker.DetectChanges();

            var entries = new
            {
                added = ChangeTracker.Entries().Where(entry => entry.State == EntityState.Added).Select(entry => entry.Entity as IEntity).ToList(),
                modified = ChangeTracker.Entries().Where(entry => entry.State == EntityState.Modified).Select(entry => entry.Entity as IEntity).ToList(),
                deleted = ChangeTracker.Entries().Where(entry => entry.State == EntityState.Deleted).Select(entry => entry.Entity as IEntity).ToList(),
            };

            foreach (ExtendedEntity<Guid> addedEntry in entries.added.Cast<ExtendedEntity<Guid>>())
            {
                addedEntry.CreatedOn = DateTime.UtcNow;
                addedEntry.CreatedBy = ResolveIdentity();
            }

            foreach (ExtendedEntity<Guid> modifiedEntry in entries.modified.Cast<ExtendedEntity<Guid>>())
            {
                modifiedEntry.ModifiedOn = DateTime.UtcNow;
                modifiedEntry.ModifiedBy = ResolveIdentity();
            }

            foreach (ExtendedEntity<Guid> deletedEntry in entries.deleted.Cast<ExtendedEntity<Guid>>())
            {
                deletedEntry.ModifiedOn = DateTime.UtcNow;
                deletedEntry.ModifiedBy = ResolveIdentity();
            }

            return base.SaveChanges();
        }

        private static Guid ResolveIdentity()
        {
            return Guid.Empty;
        }
    }
}