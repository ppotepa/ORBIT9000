using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions.Data.Entities;
using System.Reflection;

namespace ORBIT9000.Data.Context
{
    public class ExtendedDbContext : DbContext
    {
        #region Fields

        private static Type[]? _entities;
        protected static readonly SemaphoreSlim _semaphore = new(1, 1);
        protected readonly ILogger<ExtendedDbContext> _logger;
        protected readonly IConfiguration _configuration;
        private static bool _created;

        #endregion Fields

        #region Static Constructor

        static ExtendedDbContext()
        {
            _created = false;
        }

        #endregion Static Constructor

        #region Constructors

        public ExtendedDbContext(IConfiguration configuration, ILogger<ExtendedDbContext> logger)
        {
            _logger = logger;
            _configuration = configuration;

            if (!_created)
            {
                bool semaphoreAcquired = false;
                try
                {
                    semaphoreAcquired = _semaphore.Wait(TimeSpan.FromSeconds(5));
                    Database.EnsureCreated();
                    _created = true;
                }
                finally
                {
                    if (semaphoreAcquired)
                    {
                        _semaphore.Release();
                    }
                }
            }
        }

        #endregion Constructors

        #region Properties

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

        #endregion Properties

        #region Methods

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
        private static Guid ResolveIdentity()
        {
            return Guid.Empty;
        }

        #endregion Methods
    }
}