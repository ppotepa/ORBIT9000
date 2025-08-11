using Microsoft.Extensions.Logging;
using ORBIT9000.Engine;
using ORBIT9000.Engine.Builders;
using Serilog;
using System.Globalization;

namespace ORBIT9000.PoCDemo
{
    internal static class Program
    {
        private const string _outputTemplate =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

        private static void Main(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: _outputTemplate, formatProvider: CultureInfo.InvariantCulture)
                .CreateLogger();

            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(Log.Logger);
            });

            OrbitEngine engine = new OrbitEngineBuilder(loggerFactory)
                .UseConfiguration()
<<<<<<< HEAD
=======
                .UseSerilogLogging()
                .RegisterPlugins()
>>>>>>> 7611f11 (Refactor plugin loading and configuration handling)
                .Build();

            engine.Start();
        }
    }
}