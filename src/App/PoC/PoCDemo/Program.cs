using Microsoft.Extensions.Configuration;
using ORBIT9000.Core.Configuration;
using ORBIT9000.Engine;
using System.Reflection;

namespace ORBIT9000.PoCDemo
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            OrbitAppSettings? settings = config.Get<OrbitAppSettings>();

            var engine = new OrbitEngine
            (
                new()
                {
                    Plugins = settings.Engine.Plugins.ActivePlugins.Select(path => Assembly.LoadFile(path)).ToArray()
                }
            );

            engine.Execute();
       }
    }
}