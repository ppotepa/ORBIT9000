using Newtonsoft.Json;
using ORBIT9000.Engine.Runtime.State;
using System.IO.Pipes;
using System.Text;

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

            if (state.Engine.Configuration.EnableTerminal)
            {
                Task.Run(() => PipeThread(state));
            }

            state.Engine.LogInformation("Engine is running. Strategy {Strategy}", nameof(EngineStartupStrategy));

            Initialize(state.Engine);

            while (state.Engine?.IsRunning == true) // Fix for CS8602: Added null conditional operator
            {
                Execute(state.Engine);
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
        }

        private static void Execute(OrbitEngine engine)
        {
            try
            {
                var pluginTask = engine.PluginProvider.Activate("ExamplePlugin");
                var plugin2Task = engine.PluginProvider.Activate("ExamplePlugin2");

                Task.Run(async () =>
                {
                    Thread.CurrentThread.Name = "Plugin_ExamplePlugin";
                    var plugin = await pluginTask;
                    await plugin.OnLoad();
                });

                Task.Run(async () =>
                {
                    Thread.CurrentThread.Name = "Plugin_ExamplePlugin2";
                    var plugin2 = await plugin2Task;
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
            // Any initialization logic here
        }

        private static async Task PipeThread(EngineState? state)
        {
            if (state is null || state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }

            using var server = new NamedPipeServerStream("OrbitEngine", PipeDirection.Out);
          
            await server.WaitForConnectionAsync();
            state.Engine.LogInformation($"GUI Connected");

            while (state.Engine.IsRunning)
            {
                state.Engine.LogInformation($"Engine instance {state.Engine.GetHashCode()}");
                byte[] buffer = MessagePack.MessagePackSerializer.Serialize(state.ActivatedPlugins);  

                await server.WriteAsync(buffer, 0, buffer.Length);
                await Task.Delay(TimeSpan.FromMilliseconds(50));
            }
        }
    }
}
