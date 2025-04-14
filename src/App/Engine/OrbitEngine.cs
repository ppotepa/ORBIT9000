using ORBIT9000.Core.Scrapers;
using ORBIT9000.Engine.Configuration;

namespace ORBIT9000.Engine
{
    public class OrbitEngine
    {
        private readonly OrbitEngineConfig config;

        public OrbitEngine(OrbitEngineConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            //config.Plugins = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetTypes().Any(x => x.GetInterfaces().Contains(typeof(IOrbitPlugin)))).ToArray();   
        }

        public bool IsInitialized { get; private set; }

        public void Execute()
        {
            if (IsInitialized is false)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            Console.WriteLine("Scanning for Scrapers");
            foreach(var assembly in this.config.Plugins)
            {
                var scrapers = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IScraper)));
                if (scrapers.Any())
                {
                   foreach(var scraper in scrapers)
                   {
                        dynamic instance = Activator.CreateInstance(scraper);
                        instance.Execute();
                   }
                }
            }
        }
    }
}