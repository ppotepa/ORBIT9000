using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Engine;
using Serilog;

namespace ORBIT9000.PoCDemo
{
    internal class Program
    {
        private const string OutputTemplate 
            = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

        private static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            IServiceCollection services = new ServiceCollection();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo
                .Console(outputTemplate: OutputTemplate)
                .CreateLogger()                
                .ForContext<Program>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();                
                loggingBuilder.AddSerilog(Log.Logger);
            });

            services.AddSingleton<IConfiguration>(config);  

            services.AddSingleton<OrbitEngine>();

            ServiceProvider provider = services.BuildServiceProvider();
            OrbitEngine engine = provider.GetRequiredService<OrbitEngine>();

            engine.Start();
       }
    }
}