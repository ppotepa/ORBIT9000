using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Engine.State;

namespace ORBIT9000.Engine.Strategies.Running
{
    internal struct Default
    {
        public readonly static ParameterizedThreadStart DefaultEngineStrategy = static (obj) =>
        {
            if (obj is not EngineState state || state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }

            state.Engine.LogInformation("Engine is running. Strategy {Strategy}", nameof(DefaultEngineStrategy));

            while (state.Engine.IsRunning)
            {
                ProcessPlugins!(state.Engine);
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
        };

        private static readonly Action<OrbitEngine> ProcessPlugins = (engine) =>
        {
            foreach ((Type type, PluginActivationInfo pluginInfo) in engine.Plugins)
            {
                if (pluginInfo.Instances.Any() && !pluginInfo.AllowMultiple)
                {
                    engine.LogError("Plugin is already running. {Name}", type.Name);
                    continue;
                }

                engine.LogInformation("Processing Plugin: {Name}", type.Name);

                using var scope = engine.ServiceProvider.CreateAsyncScope();
                Task task = Task.Run(async () =>
                {
                    await using var scope = engine.ServiceProvider.CreateAsyncScope();

                    if (scope.ServiceProvider.GetService(type) is not IOrbitPlugin instance)
                    {
                        engine.LogError("Failed to resolve plugin instance for {Name}.", type.Name);
                        return;
                    }

                    await instance.OnLoad();
                });

                pluginInfo.Instances.Add(task);

                task.ContinueWith(completed =>
                {
                    engine.LogInformation("Plugin {Name} has finished running.", type.Name);
                    pluginInfo.Instances.Remove(completed);
                });
            }
        };
    }
}
