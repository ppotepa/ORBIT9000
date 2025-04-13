using ORBIT9000.Core.Plugin;

namespace ORBIT9000.Core.Scrapers
{
    public abstract class GenericScraperBase : IScraper
    {
        protected GenericScraperBase()
        {
            this.Scrapers = [.. this.GetType().GetGenericArguments().Select(t => Activator.CreateInstance(t) as IScraper)];
        }

        public IScraper[] Scrapers { get; }

        public abstract void Execute();
    }

    public abstract class GenericScraper<T1> : GenericScraperBase
    where T1 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5, T6> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
        where T6 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5, T6, T7> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
        where T6 : IScraper
        where T7 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5, T6, T7, T8> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
        where T6 : IScraper
        where T7 : IScraper
        where T8 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5, T6, T7, T8, T9> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
        where T6 : IScraper
        where T7 : IScraper
        where T8 : IScraper
        where T9 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
        where T6 : IScraper
        where T7 : IScraper
        where T8 : IScraper
        where T9 : IScraper
        where T10 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
        where T6 : IScraper
        where T7 : IScraper
        where T8 : IScraper
        where T9 : IScraper
        where T10 : IScraper
        where T11 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
        where T6 : IScraper
        where T7 : IScraper
        where T8 : IScraper
        where T9 : IScraper
        where T10 : IScraper
        where T11 : IScraper
        where T12 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
        where T6 : IScraper
        where T7 : IScraper
        where T8 : IScraper
        where T9 : IScraper
        where T10 : IScraper
        where T11 : IScraper
        where T12 : IScraper
        where T13 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
        where T6 : IScraper
        where T7 : IScraper
        where T8 : IScraper
        where T9 : IScraper
        where T10 : IScraper
        where T11 : IScraper
        where T12 : IScraper
        where T13 : IScraper
        where T14 : IScraper
    {
    }

    public abstract class GenericScraper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : GenericScraperBase
        where T1 : IScraper
        where T2 : IScraper
        where T3 : IScraper
        where T4 : IScraper
        where T5 : IScraper
        where T6 : IScraper
        where T7 : IScraper
        where T8 : IScraper
        where T9 : IScraper
        where T10 : IScraper
        where T11 : IScraper
        where T12 : IScraper
        where T13 : IScraper
        where T14 : IScraper
        where T15 : IScraper
    {
    }
}
