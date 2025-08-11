<<<<<<< HEAD
﻿using Microsoft.Extensions.Logging;
using ORBIT9000.Engine;
using ORBIT9000.Engine.Builders;
using Serilog;
using System.Globalization;
=======
﻿using ORBIT9000.Engine;
using ORBIT9000.Engine.Builders;
using ORBIT9000.Plugins.Twitter;
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)

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
<<<<<<< HEAD
                .RegisterPlugins()
>>>>>>> 7611f11 (Refactor plugin loading and configuration handling)
=======
                .RegisterPlugins(typeof(TwitterPlugin))
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
                .Build();

            engine.Start();
        }
    }
}