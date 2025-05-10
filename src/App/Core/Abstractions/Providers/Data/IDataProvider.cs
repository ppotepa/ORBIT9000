namespace ORBIT9000.Core.Abstractions.Providers.Data
{
    public interface IDataProvider;
    public interface IDataProvider<TResult> : IDataProvider
        where TResult : IResult, new()
    {
        Task<IEnumerable<TResult>> GetData();
    }
}
