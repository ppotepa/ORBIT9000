using ORBIT9000.Abstractions.Data.Entities;

namespace ORBIT9000.Abstractions.Data
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        IQueryable<TEntity> GetAll();

        TEntity? FindById(params object[] key);

        void Add(TEntity entity);

        void Remove(TEntity entity);

        void Save();
    }
}