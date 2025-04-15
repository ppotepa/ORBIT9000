using ORBIT9000.Core.Plugin;

namespace ORBIT9000.Core.Scrapers
{
    //public abstract class AggregateScraperBase<TResult> : IAggregateScraper<TResult>
    //    where TResult : new()
    //{
    //    public Type[] Scrapers;

    //    public AggregateScraperBase()
    //        => this.Scrapers ??= this.GetType()?.BaseType?.GetGenericArguments().Where(t => t.GetInterfaces().Contains(typeof(IScraper))).ToArray() ?? [];

    //    public TResult[] Scrape()
    //    {
    //        return [.. this.Scrapers.Select(type => {
    //            dynamic instance = Activator.CreateInstance(type);
    //            return instance.Scrape();
    //        })];
    //    }

    //    TResult IScraper<TResult>.Scrape()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public abstract class AggregateScraper<T1, TResult> : AggregateScraperBase<TResult>
    //    where T1 : IScraper<TResult>
    //    where TResult : new()
    //{
    //}

    //public abstract class AggregateScraper<T1, T2, TResult> : AggregateScraperBase<TResult>
    //    where T1 : IScraper<TResult>
    //    where T2 : IScraper<TResult>
    //    where TResult : new()
    //{
    //}   
}