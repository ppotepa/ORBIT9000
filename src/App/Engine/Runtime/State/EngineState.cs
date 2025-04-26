using ORBIT9000.Abstractions;
using ORBIT9000.Core.Events;

namespace ORBIT9000.Engine.Runtime.State
{
    public class EngineState
    {
        private readonly OrbitEngine _engine;
        private readonly IPluginProvider _pluginProvider;
        public EngineState(IPluginProvider pluginProvider, OrbitEngine engine)
        {
            _pluginProvider = pluginProvider;
            _engine = engine;

            ListenToPluginEvents();
        }

        public List<Type> ActivePlugins { get; internal set; } = new List<Type>();
        public OrbitEngine? Engine { get => _engine; }
        private void HandlePluginEvent(PluginEvent pluginEvent)
        {
            switch (pluginEvent.Type)
            {
                case PluginEventType.Activated:
                    var activatedPlugin = Type.GetType(pluginEvent.PluginName);
                    if (activatedPlugin != null && !ActivePlugins.Contains(activatedPlugin))
                    {
                        ActivePlugins.Add(activatedPlugin);
                    }
                    break;

                case PluginEventType.Deactivated:
                    var deactivatedPlugin = Type.GetType(pluginEvent.PluginName);
                    if (deactivatedPlugin != null)
                    {
                        ActivePlugins.Remove(deactivatedPlugin);
                    }
                    break;
            }
        }

        private void ListenToPluginEvents()
        {
            _ = Task.Run(async () =>
            {
                await foreach (var pluginEvent in _pluginProvider.PluginEvents.ReadAllAsync())
                {
                    HandlePluginEvent(pluginEvent);
                }
            });
        }
    }
}