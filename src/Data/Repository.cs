using ORBIT9000.Core.Abstractions.Data;
using ORBIT9000.Core.Abstractions.Data.Entities;

namespace ORBIT9000.Data
{
    public class Repository<TEntity>(IDbAdapter adapter) : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly IDbAdapter _adapter = adapter;

        public IQueryable<TEntity> GetAll()
        {
            return this._adapter.Query<TEntity>();
        }

        public TEntity? FindById(params object[] key)
        {
            return this._adapter.Find<TEntity>(key);
        }

        public void Add(TEntity entity)
        {
            this._adapter.Add(entity);
        }

        public void Remove(TEntity entity)
        {
            this._adapter.Remove(entity);
        }

        public void Save()
        {
            this._adapter.SaveChanges();
        }

        public IQueryable<TEntity> Query()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> GetAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}