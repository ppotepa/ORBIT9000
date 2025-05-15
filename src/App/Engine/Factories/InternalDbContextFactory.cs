using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ORBIT9000.Data.Context;
using System.Collections.Concurrent;

namespace ORBIT9000.Engine.Factories
{
    internal class InternalDbContextFactory
    {
        private readonly IComponentContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<InternalDbContextFactory> _logger;
        private readonly ConcurrentDictionary<Type, ExtendedDbContext> _contextCache = new();

        public InternalDbContextFactory(IComponentContext context, IConfiguration configuration,
            ILogger<InternalDbContextFactory> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public DbContext ResolveContext(string? scope = "")
        {
            Type type = typeof(LocalDbContext);

            // If a scope is specified, try to resolve from cache using scope as key
            if (!string.IsNullOrEmpty(scope))
            {
                string scopedKey = $"{type.FullName}_{scope}";
                if (!_contextCache.TryGetValue(scopedKey.GetType(), out ExtendedDbContext? dbContext))
                {
                    _logger.LogInformation("DbContext of type {DbContextType} with scope '{Scope}' not found in cache. Creating new instance.", type.Name, scope);
                    dbContext = CreateDbContext<LocalDbContext>(type);
                    _contextCache[scopedKey.GetType()] = dbContext;
                    _logger.LogInformation("DbContext of type {DbContextType} with scope '{Scope}' created and added to cache.", type.Name, scope);
                }
                else
                {
                    _logger.LogInformation("DbContext of type {DbContextType} with scope '{Scope}' retrieved from cache.", type.Name, scope);
                }
                return dbContext;
            }
            else
            {
                // No scope specified, always create a new instance
                _logger.LogInformation("No scope specified. Creating new DbContext of type {DbContextType}.", type.Name);
                return CreateDbContext<LocalDbContext>(type);
            }
        }

        private ExtendedDbContext CreateDbContext<TContext>(Type type) where TContext : ExtendedDbContext
        {
            _logger.LogDebug("Attempting to create DbContext of type {DbContextType}.", type.Name);
            if (type == typeof(LocalDbContext))
            {
                LocalDbContext context = _context.Resolve<LocalDbContext>();
                _logger.LogDebug("Resolved LocalDbContext from Autofac container.");
                return context;
            }
            else if (type == typeof(ReflectiveInMemoryContext))
            {
                _logger.LogDebug("Creating new instance of ReflectiveInMemoryContext.");
                return new ReflectiveInMemoryContext(null, null);
            }
            else
            {
                _logger.LogError("DbContext of type {DbContextType} is not supported.", type.Name);
                throw new NotSupportedException($"DbContext of type {type.Name} is not supported.");
            }
        }
    }
}
