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

            Initialize!(state.Engine);

            while (state.Engine.IsRunning)
            {
                Execute!(state.Engine);

                Thread.Sleep(TimeSpan.FromMilliseconds(150));
            }
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
