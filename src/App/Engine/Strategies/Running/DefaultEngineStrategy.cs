using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Engine.State;

namespace ORBIT9000.Engine.Strategies.Running
{
    internal struct Default
    {
        public readonly static ParameterizedThreadStart DefaultEngineStrategy = static (obj) =>
        {
            var state = obj as EngineState;

            if (state is null || state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }

            state.Engine._logger.LogInformation("Engine is running. Strategy {Strategy}", nameof(DefaultEngineStrategy));

            while (state.Engine.IsRunning)
            {
                ProcessPlugins!(state.Engine);
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        };

        private static readonly Action<OrbitEngine> ProcessPlugins = (engine) =>
        {
            foreach (var (type, pluginInfo) in engine._plugins)
            {
                engine._logger.LogInformation("Processing Plugin: {Name}", type.Name);

                using var scope = engine._serviceProvider.CreateAsyncScope();
                engine._logger.LogInformation("Creating scope for {Name}", type.Name);

                if (pluginInfo.Instances.Any() && !pluginInfo.AllowMultiple)
                {
                    engine._logger.LogWarning("Plugin is already running. {Name}", type.Name);
                    continue;
                }

                Task task = Task.Run(async () =>
                {
                    await using var scope = engine._serviceProvider.CreateAsyncScope();

                    if (scope.ServiceProvider.GetService(type) is not IOrbitPlugin instance)
                    {
                        engine._logger.LogError("Failed to resolve plugin instance for {Name}.", type.Name);
                        return;
                    }

                    await instance.Run();
                });

                pluginInfo.Instances.Add(task);

                task.ContinueWith(completed =>
                {
                    engine._logger.LogInformation("Plugin {Name} has finished running.", type.Name);
                    pluginInfo.Instances.Remove(completed);
                });
            }
        };
    }
}
