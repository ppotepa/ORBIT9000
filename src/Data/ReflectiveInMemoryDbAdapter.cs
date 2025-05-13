using ORBIT9000.Core.Abstractions.Data.Entities;
using ORBIT9000.Data.Context;
using ORBIT9000.Data.ORBIT9000.Data.Context;

namespace ORBIT9000.Data
{
    public class ReflectiveInMemoryDbAdapter(ReflectiveInMemoryContext context) : IDbAdapter
    {
        #region Fields

        private readonly ReflectiveInMemoryContext _context = context;

        #endregion Fields

        #region Methods

        public void Add<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            this._context.Set<TEntity>().Add(entity);
        }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity
        {
            this._context.Set<TEntity>().AddRange(entities);
        }

        public void Attach<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            this._context.Set<TEntity>().Attach(entity);
        }

        public bool Exists<TEntity>(Func<TEntity, bool> predicate) where TEntity : class, IEntity
        {
            return this._context.Set<TEntity>().Any(predicate);
        }

        public TEntity? Find<TEntity>(params object[] keyValues) where TEntity : class, IEntity
        {
            return this._context.Set<TEntity>().Find(keyValues);
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity
        {
            return this._context.Set<TEntity>();
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            this._context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity
        {
            foreach (TEntity entity in entities)
            {
                this._context.Set<TEntity>().Remove(entity);
            }
        }

        public int SaveChanges()
        {
            return this._context.SaveChanges();
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            this._context.Set<TEntity>().Update(entity);
        }

        #endregion Methods
    }
}