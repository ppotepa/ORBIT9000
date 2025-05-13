using ORBIT9000.Core.Abstractions.Data.Entities;

namespace ORBIT9000.Data
{

    public interface IDbAdapter
    {
        #region Methods

        void Add<TEntity>(TEntity entity) where TEntity : class, IEntity;

        void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity;

        void Attach<TEntity>(TEntity entity) where TEntity : class, IEntity;

        bool Exists<TEntity>(Func<TEntity, bool> predicate) where TEntity : class, IEntity;

        TEntity? Find<TEntity>(params object[] keyValues) where TEntity : class, IEntity;

        IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity;

        void Remove<TEntity>(TEntity entity) where TEntity : class, IEntity;

        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity;

        int SaveChanges();

        void Update<TEntity>(TEntity entity) where TEntity : class, IEntity;

        #endregion Methods
    }
}