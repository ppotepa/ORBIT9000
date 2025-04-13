namespace ORBIT9000.Core.Plugin
{
    public class OrbitPlugin
    {
    }

    public interface IOrbitPlugin<TInstaller, TScraper> 
       where TInstaller : IInstaller, new()
       where TScraper : IScraper, new()

    {
        TInstaller Installer => new();
        TScraper Scraper => new();
    }
}
