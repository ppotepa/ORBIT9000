using ORBIT9000.Core.Abstractions.Result;

namespace ORBIT9000.Core.Abstractions.Data
{
    public interface IDataProvider { }
    public interface IDataProvider<out TResult> : IDataProvider
        where TResult : IResult, new()
    {
        IEnumerable<TResult> GetData();
    }
}
