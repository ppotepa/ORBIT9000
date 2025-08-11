<<<<<<< HEAD
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
=======
﻿using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine;
using ORBIT9000.Engine.Builders;
using ORBIT9000.Plugins.Twitter;
using Serilog;
>>>>>>> 9aa9371 (Replace Serilog with Microsoft.Extensions.Logging)

namespace ORBIT9000.PoCDemo
{
    internal static class Program
    {
        private const string _outputTemplate =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

        private static void Main(string[] args)
        {
<<<<<<< HEAD
<<<<<<< HEAD
            ArgumentNullException.ThrowIfNull(args);

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: _outputTemplate, formatProvider: CultureInfo.InvariantCulture)
                .CreateLogger();

            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
=======
=======
            var a = typeof(IOrbitPlugin).IsAssignableFrom(typeof(TwitterPlugin));
>>>>>>> a1c6c63 (Refactor plugin architecture and configuration handling)
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: _outputTemplate)
                .CreateLogger();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
>>>>>>> 9aa9371 (Replace Serilog with Microsoft.Extensions.Logging)
            {
                builder.ClearProviders();
                builder.AddSerilog(Log.Logger);
            });

            OrbitEngine engine = new OrbitEngineBuilder(loggerFactory)
                .UseConfiguration()
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
                .UseSerilogLogging()
<<<<<<< HEAD
                .RegisterPlugins()
>>>>>>> 7611f11 (Refactor plugin loading and configuration handling)
=======
=======
>>>>>>> 9aa9371 (Replace Serilog with Microsoft.Extensions.Logging)
                .RegisterPlugins(typeof(TwitterPlugin))
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
=======
>>>>>>> a1c6c63 (Refactor plugin architecture and configuration handling)
                .Build();

            engine.Start();
        }
    }
}