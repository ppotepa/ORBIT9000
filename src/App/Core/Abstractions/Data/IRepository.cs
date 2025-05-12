using ORBIT9000.Core.Abstractions.Data.Entities;

namespace ORBIT9000.Data
{
    public interface IRepository<TEntity>
        where TEntity : IEntity
    {
        IQueryable<TEntity> Query();
        Task<TEntity?> GetAsync(object id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
