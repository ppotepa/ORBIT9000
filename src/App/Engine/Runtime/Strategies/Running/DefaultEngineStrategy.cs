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
                Task.Run(() => PipeThread(state));
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

        private static async Task PipeThread(EngineState? state)
        {
            if (state is null || state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }

            state.Engine.LogInformation("Starting PipeThread.");

            using var server = new NamedPipeServerStream("OrbitEngine", PipeDirection.Out);

            await server.WaitForConnectionAsync();
            state.Engine.LogInformation($"GUI Connected");

            var random = new Random();

            while (state.Engine.IsRunning)
            {
                var options = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
                    ContractlessStandardResolver.Instance,
                    StandardResolver.Instance
                ));

                var exampleData = new ExampleData
                {
                    Frame1 = new SettingsData
                    {
                        Setting1 = random.Next(1, 100),
                        Setting2 = "Text2",
                    },

                    Frame2 = new EngineData
                    {
                        Setting1 = random.Next(1, 100),
                        Setting2 = random.Next(1, 100)
                    }
                };

                byte[] buffer = MessagePack.MessagePackSerializer.Serialize(exampleData, options);

                
                if (random.NextDouble() > 0.97) 
                {
                    await server.WriteAsync(buffer, 0, buffer.Length);
                    state.Engine.LogDebug("Message sent to GUI.");
                }
                else
                {
                    state.Engine.LogDebug("Message skipped for this interval.");
                }

                await Task.Delay(TimeSpan.FromMilliseconds(random.Next(50, 200))); 
            }

            state.Engine.LogInformation("PipeThread has completed.");
        }
    }
}
