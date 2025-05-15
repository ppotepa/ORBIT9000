using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ORBIT9000.Data.Context;
using System.Collections.Concurrent;

namespace ORBIT9000.Engine.Factories
{
    internal class InternalDbContextFactory
    {
        private readonly IComponentContext _context;
        private readonly IConfiguration _configuration;

        private readonly ConcurrentDictionary<Type, ExtendedDbContext> _contextCache = new();

        public InternalDbContextFactory(IComponentContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public DbContext ResolveContext()
        {
            return _context.Resolve<LocalDbContext>();
        }

        private ExtendedDbContext CreateDbContext<TContext>(Type type) where TContext : ExtendedDbContext
        {
            if (type == typeof(LocalDbContext))
            {
                return new LocalDbContext(_configuration);
            }
            else if (type == typeof(ReflectiveInMemoryContext))
            {
                return new ReflectiveInMemoryContext();
            }
            else
            {
                throw new NotSupportedException($"DbContext of type {type.Name} is not supported.");
            }
        }
    }
}
