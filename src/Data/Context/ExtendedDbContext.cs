using Microsoft.EntityFrameworkCore;
using ORBIT9000.Abstractions.Data.Entities;
using System.Reflection;

namespace ORBIT9000.Data.Context
{
    public class ExtendedDbContext : DbContext
    {
        private static Type[]? _entities;

        private const BindingFlags PRIVATE_FIELD_BINDING_ATTRS = BindingFlags.Instance |
            BindingFlags.NonPublic | BindingFlags.IgnoreCase;

        private static IEnumerable<Type> Entities
        {
            get
            {
                //HACK: for some reason entities are loaded twice.
                _entities ??= [..AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes()
                        .Where(type => type.GetInterfaces().Contains(typeof(IExtendedEntity<Guid>))))
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
                modelBuilder.Entity(entity).HasKey(nameof(IExtendedEntity<byte>.Id));

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
            ChangeTracker.DetectChanges();

            var entries = new
            {
                added = ChangeTracker.Entries().Where(entry => entry.State == EntityState.Added).Select(entry => entry.Entity as IEntity).ToList(),
                modified = ChangeTracker.Entries().Where(entry => entry.State == EntityState.Modified).Select(entry => entry.Entity as IEntity).ToList(),
                deleted = ChangeTracker.Entries().Where(entry => entry.State == EntityState.Deleted).Select(entry => entry.Entity as IEntity).ToList(),
            };

            foreach (IEntity? addedEntry in entries.added)
            {
                typeof(ExtendedEntity<Guid>).GetField(nameof(IExtendedEntity<Guid>.CreatedOn), PRIVATE_FIELD_BINDING_ATTRS)?.SetValue(addedEntry, DateTime.Now);
                typeof(ExtendedEntity<Guid>).GetField(nameof(IExtendedEntity<Guid>.CreatedBy), PRIVATE_FIELD_BINDING_ATTRS)?.SetValue(addedEntry, Guid.NewGuid());
            }

            foreach (IEntity? modifiedEntry in entries.modified)
            {
                typeof(ExtendedEntity<Guid>).GetField(nameof(IExtendedEntity<Guid>.ModifiedOn), PRIVATE_FIELD_BINDING_ATTRS)?.SetValue(modifiedEntry, DateTime.Now);
                typeof(ExtendedEntity<Guid>).GetField(nameof(IExtendedEntity<Guid>.ModifiedBy), PRIVATE_FIELD_BINDING_ATTRS)?.SetValue(modifiedEntry, Guid.NewGuid());
            }

            foreach (IEntity? deletedEntry in entries.deleted)
            {
                typeof(ExtendedEntity<Guid>).GetField(nameof(IExtendedEntity<Guid>.DeletedOn), PRIVATE_FIELD_BINDING_ATTRS)?.SetValue(deletedEntry, DateTime.Now);
                typeof(ExtendedEntity<Guid>).GetField(nameof(IExtendedEntity<Guid>.DeletedBy), PRIVATE_FIELD_BINDING_ATTRS)?.SetValue(deletedEntry, Guid.NewGuid());
            }

            return base.SaveChanges();
        }
    }
}