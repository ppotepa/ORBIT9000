using ORBIT9000.Core;

namespace ORBIT9000.Core
{
    //public abstract class AggregateScraperBase<TResult> : IAggregateScraper<TResult>
    //    where TResult : new()
    //{
    //    public Type[] DataProviders;

    //    public AggregateScraperBase()
    //        => this.DataProviders ??= this.GetType()?.BaseType?.GetGenericArguments().Where(t => t.GetInterfaces().Contains(typeof(IDataProvider))).ToArray() ?? [];

    //    public TResult[] GetData()
    //    {
    //        return [.. this.DataProviders.Select(type => {
    //            dynamic instance = Activator.CreateInstance(type);
    //            return instance.GetData();
    //        })];
    //    }

    //    TResult IDataProvider<TResult>.GetData()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public abstract class AggregateScraper<T1, TResult> : AggregateScraperBase<TResult>
    //    where T1 : IDataProvider<TResult>
    //    where TResult : new()
    //{
    //}

    //public abstract class AggregateScraper<T1, T2, TResult> : AggregateScraperBase<TResult>
    //    where T1 : IDataProvider<TResult>
    //    where T2 : IDataProvider<TResult>
    //    where TResult : new()
    //{
    //}   
}