using ORBIT9000.Core.Result;

namespace ORBIT9000.Core.Scrapers
{
    public interface IScraper { }
    public interface IScraper<out TResult> : IScraper
        where TResult : IScrapeResult, new()
    {
        TResult Scrape();
    }
}
