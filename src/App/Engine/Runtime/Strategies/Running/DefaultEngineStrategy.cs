using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine.Runtime.State;

namespace ORBIT9000.Engine.Strategies.Running
{
    internal struct Default
    {
        public readonly static ParameterizedThreadStart EngineStartupStrategy = static (obj) =>
        {
            IServiceProvider pluginScope = default;

            if (obj is not EngineState state || state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }

            state.Engine.LogInformation("Engine is running. Strategy {Strategy}", nameof(EngineStartupStrategy));

            LoadPlugins!(state.Engine);

            while (state.Engine.IsRunning)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
        };

        private static readonly Action<OrbitEngine> LoadPlugins = async (engine) =>
        {
            var plugin = engine.PluginProvider.Activate("TwitterPlugin") as IOrbitPlugin;
            await plugin.OnLoad();
        };

        public Default()
        {
        }

        //private static readonly Action<OrbitEngine> LoadPlugins = async (engine) =>
        //{
        //    foreach (Loaders.PluginRegistrationInfo plugin in PluginProvider.GetPluginRegistrationInfo())
        //    {
        //        if (plugin.Tasks.Any() && !plugin.AllowMultiple)
        //        {
        //            engine.LogError("Plugin is already running. {Name}", plugin.Type.Name);
        //            continue;
        //        }

        //        engine.LogInformation("Processing Plugin: {Name}", plugin.Type.Name);

        //        using var scope = engine.ServiceProvider.CreateAsyncScope();
        //        Task task = Task.Run(async () =>
        //        {
        //            await using var scope = engine.ServiceProvider.CreateAsyncScope();
        //            IOrbitPlugin? instance = scope.ServiceProvider.GetService(plugin.Type) as IOrbitPlugin;
        //            await instance.OnLoad();
        //        });

        //        plugin.Tasks.Add(task);

        //        await task.ContinueWith(completed =>
        //        {
        //            engine.LogInformation("Plugin {Name} has finished running.", plugin.Type.Name);
        //            plugin.Tasks.Remove(completed);
        //        });
        //    }
        //};
    }
}
