using MessagePack.Resolvers;
using MessagePack;
using ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Engine.Runtime.State;
using System.IO.Pipes;

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
                engine.LogDebug("Executing plugin activation tasks.");

                var pluginTask = engine.PluginProvider.Activate("ExamplePlugin");
                var plugin2Task = engine.PluginProvider.Activate("ExamplePlugin2");

                Task.Run(async () =>
                {
                    Thread.CurrentThread.Name = "Plugin_ExamplePlugin";

                    var plugin = await pluginTask;

                    engine.LogDebug("Plugin ExamplePlugin loaded.");
                    await plugin.OnLoad();
                });

                Task.Run(async () =>
                {
                    Thread.CurrentThread.Name = "Plugin_ExamplePlugin2";

                    var plugin2 = await plugin2Task;

                    engine.LogDebug("Plugin ExamplePlugin2 loaded.");

                    await plugin2.OnLoad();
                });
            }
            catch (Exception ex)
            {
                engine.LogError("An error occurred while loading plugins: {Message}", ex.Message);
            }
        }

        private static void Initialize(OrbitEngine engine)
        {
            engine.LogInformation("Initializing engine.");
            // Any initialization logic here
            engine.LogInformation("Engine initialization completed.");
        }
    }
}
