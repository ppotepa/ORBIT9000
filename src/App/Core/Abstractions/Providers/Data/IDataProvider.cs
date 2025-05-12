using ORBIT9000.Core.Abstractions.Data.Entities;

namespace ORBIT9000.Core.Abstractions.Providers.Data
{
    public interface IDataProvider;
    public interface IDataProvider<TResult> : IDataProvider
        where TResult : IEntity, new()

    {
        Task<IEnumerable<TResult>> GetData();
    }
}
