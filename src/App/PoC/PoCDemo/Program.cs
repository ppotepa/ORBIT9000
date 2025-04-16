using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ORBIT9000.Engine;
using Serilog;
using Serilog.Extensions.Logging;

namespace ORBIT9000.PoCDemo
{
    internal class Program
    {
        private const string OutputTemplate 
            = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo
                .Console(outputTemplate: OutputTemplate)
                .CreateLogger()                
                .ForContext<Program>();

            var loggerFactory = new LoggerFactory(new[] {
                new SerilogLoggerProvider(Log.Logger)
            });

            Log.Logger.Information("Logger created.");

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            Log.Logger.Information("Loaded configuration");

            //var engine = new Engine.OrbitEngine(config, loggerFactory.CreateLogger<OrbitEngine>());
            OrbitEngine engine = new OrbitEngine(config!, loggerFactory.CreateLogger<OrbitEngine>());

            Log.Logger.Information($"Orbit engine create with configuration : {config}");
            Log.Logger.Information($"Starting engine.");

            engine.Start();
       }
    }
}