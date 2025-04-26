using Newtonsoft.Json;
using ORBIT9000.Engine.Runtime.State;
using System.IO.Pipes;
using System.Text;

namespace ORBIT9000.Engine.Strategies.Running
{
    internal struct Default
    {
        public readonly static ParameterizedThreadStart EngineStartupStrategy = static (obj) =>
        {
            if (obj is not EngineState state || state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }

            if(state.Engine.Configuration.EnableTerminal is true)
            {
                var pipeThread = new Thread(PipeThread!);
                pipeThread.Start(obj);
            }

            state.Engine.LogInformation("Engine is running. Strategy {Strategy}", nameof(EngineStartupStrategy));

            Initialize!(state.Engine);

            while (state.Engine.IsRunning)
            {
                Execute!(state.Engine);

                Thread.Sleep(TimeSpan.FromMilliseconds(150));
            }
        };

        public readonly static ParameterizedThreadStart PipeThread = async static (obj) =>
        {

            if (obj is not EngineState state || state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }

            var server = new NamedPipeServerStream("OrbitEngine", PipeDirection.Out);
            Console.WriteLine("Waiting for GUI to connect...");
            await server.WaitForConnectionAsync();
            Console.WriteLine("GUI connected!");


            while (state.Engine.IsRunning) 
            {
                var message = new
                {
                    state = state.Engine
                };

                string json = JsonConvert.SerializeObject(message, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
                byte[] buffer = Encoding.UTF8.GetBytes(json);

                await server.WriteAsync(buffer, 0, buffer.Length);
                await Task.Delay(TimeSpan.FromMilliseconds(50));
            }

            server.Dispose();
        };

        private static readonly Action<OrbitEngine> Execute = async (engine) =>
        {
            try
            {
                var plugin = engine.PluginProvider.Activate("ExamplePlugin");
                var plugin2 = engine.PluginProvider.Activate("ExamplePlugin2");

                await plugin.OnLoad();
                await plugin2.OnLoad();
            }
            catch (Exception ex)
            {
                engine.LogError("An error occurred while loading plugins: {Message}", ex.Message);
            }
        };

        private static readonly Action<OrbitEngine> Initialize = (engine) =>
        {
        };

        public Default()
        {
        }
    }
}
