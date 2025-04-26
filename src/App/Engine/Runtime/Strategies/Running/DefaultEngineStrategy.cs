using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine.Runtime.State;

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

            state.Engine.LogInformation("Engine is running. Strategy {Strategy}", nameof(EngineStartupStrategy));
            

            while (state.Engine.IsRunning)
            {
                LoadPlugins!(state.Engine);
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            }
        };

        private static readonly Action<OrbitEngine> LoadPlugins = async (engine) =>
        {
            var plugin = engine.PluginProvider.Activate("ExamplePlugin");
            await plugin.OnLoad();
        };

        public Default()
        {
        }
    }
}
