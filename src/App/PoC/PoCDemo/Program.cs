using Microsoft.Extensions.Configuration;
using ORBIT9000.Core.Configuration;
using ORBIT9000.Engine;
using ORBIT9000.Engine.Configuration;
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
            
            var engine = new Engine.OrbitEngine(config);

            engine.Execute();
       }
    }
}