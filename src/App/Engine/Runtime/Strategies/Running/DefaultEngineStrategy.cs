using Newtonsoft.Json;
using ORBIT9000.Engine.Runtime.State;
using System.IO.Pipes;
using System.Text;

namespace ORBIT9000.Engine.Strategies.Running
{
    internal static class Default
    {
        public static void EngineStartupStrategy(object obj)
        {
            if (obj is not EngineState state || state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }

            if (state.Engine.Configuration.EnableTerminal)
            {
                // Start PipeThread correctly
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
                var plugin = engine.PluginProvider.Activate("ExamplePlugin");
                var plugin2 = engine.PluginProvider.Activate("ExamplePlugin2");

                Task.Run(plugin.OnLoad);
                Task.Run(plugin2.OnLoad);
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

            Console.WriteLine("Waiting for GUI to connect...");
            await server.WaitForConnectionAsync();
            Console.WriteLine("GUI connected!");

            while (state.Engine.IsRunning) 
            {
                var message = new
                {
                    state = state.Engine
                };

                string json = JsonConvert.SerializeObject(message, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                byte[] buffer = Encoding.UTF8.GetBytes(json);

                await server.WriteAsync(buffer, 0, buffer.Length);
                await Task.Delay(TimeSpan.FromMilliseconds(50));
            }
        }
    }
}
