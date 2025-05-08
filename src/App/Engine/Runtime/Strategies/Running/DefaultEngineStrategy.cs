using MessagePack.Resolvers;
using MessagePack;
using ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Engine.Runtime.State;
using System.IO.Pipes;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Core.Parsing;

namespace ORBIT9000.Engine.Strategies.Running
{
    internal static class Default
    {
        public static void EngineStartupStrategy(object? obj)
        {
            if (obj is not EngineState state || state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }

            state.Engine.LogInformation("Starting EngineStartupStrategy.");

            if (state.Engine.Configuration.EnableTerminal)
            {
                Task.Run(() => new PipeThreadHandler(state).StartAsync());
            }

            state.Engine.LogInformation("Engine is running. Strategy {Strategy}", nameof(EngineStartupStrategy));

            Initialize(state.Engine);

            while (state.Engine?.IsRunning == true)
            {
                state.Engine.LogDebug("Engine loop iteration started.");

                Execute(state.Engine);

                state.Engine.LogDebug("Engine loop iteration completed.");
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            }

            state.Engine.LogInformation("EngineStartupStrategy has completed.");
        }

        private static void Execute(OrbitEngine engine)
        {
            try
            {
                foreach(var plugin in engine.PluginProvider.Plugins)
                {
                    engine.PluginProvider.Activate(plugin);
                }
            }
            catch (Exception ex)
            {
                engine.LogError("An error occurred while loading plugins: {Message}", ex.Message);
            }
        }

        private static void Initialize(OrbitEngine engine)
        {
            var parser = new TextScheduleParser();

            try
            {
                engine.LogInformation("Initializing plugins with scheduled jobs.");

                foreach (var pluginType in engine.PluginProvider.Plugins)
                {
                    var scheduleJobAttribute = pluginType.GetCustomAttributes(typeof(SchedulableService), inherit: true).FirstOrDefault();
                    
                    if (scheduleJobAttribute is SchedulableService jobAttribute)
                    {
                        var job = parser.Parse(jobAttribute.ScheduleExpression);
                        engine.LogInformation("Found scheduled job in plugin: {PluginType}, Schedule: {Schedule}", pluginType.Name, jobAttribute.ScheduleExpression);
                        engine.Scheduler.Schedule(job, () => { Console.WriteLine("THIS IS SCHEDULED JOB"); });
                    }
                }

                engine.LogInformation("Plugin initialization completed.");
            }
            catch (Exception ex)
            {
                engine.LogError("An error occurred during plugin initialization: {Message}", ex.Message);
            }
        }
    }
}
