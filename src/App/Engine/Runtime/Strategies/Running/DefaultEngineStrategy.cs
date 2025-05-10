using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Core.TempTools;
using ORBIT9000.Engine.Runtime.Pipe;
using ORBIT9000.Engine.Runtime.State;

namespace ORBIT9000.Engine.Runtime.Strategies.Running
{
    internal static class Default
    {
        public static void EngineStartupStrategy(object? obj)
        {
            if (obj is not EngineState { Engine: { } engine })
            {
                throw new InvalidOperationException("Engine state is null.");
            }

            EngineState? state = obj as EngineState;

            engine.LogInformation("Starting EngineStartupStrategy.");

            if (engine.Configuration.EnableTerminal)
            {
                Task.Run(() => new PipeThreadHandler(state!).StartAsync());
            }

            engine.LogInformation("Engine is running. Strategy {Strategy}", nameof(EngineStartupStrategy));

            Initialize(engine);

            while (engine.IsRunning)
            {
                engine.LogDebug("Engine loop iteration started.");

                Execute(engine);

                engine.LogDebug("Engine loop iteration completed.");
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            }

            engine.LogInformation("EngineStartupStrategy has completed.");
        }

        private static void Execute(OrbitEngine engine)
        {
            try
            {
                foreach (Type plugin in engine.PluginProvider.Plugins)
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
            ITextScheduleParser parser = engine.ServiceProvider.GetService<ITextScheduleParser>()
                ?? throw new InvalidOperationException("Job parser is not available.");

            try
            {
                engine.LogInformation("Initializing plugins with scheduled jobs.");

                foreach (Type pluginType in engine.PluginProvider.Plugins)
                {
                    object? scheduleJobAttribute = pluginType.GetCustomAttributes(typeof(SchedulableServiceAttribute), inherit: true).FirstOrDefault();

                    if (scheduleJobAttribute is SchedulableServiceAttribute jobAttribute)
                    {
                        IScheduleJob job = parser.Parse(jobAttribute.ScheduleExpression);
                        engine.LogInformation("Found scheduled job in plugin: {PluginType}, Schedule: {Schedule}", pluginType.Name, jobAttribute.ScheduleExpression);
                        engine.Scheduler.Schedule(job);
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