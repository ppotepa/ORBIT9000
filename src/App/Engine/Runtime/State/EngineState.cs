using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Abstractions.Runtime;

namespace ORBIT9000.Engine.Runtime.State
{
    public class EngineState
    {
        private readonly GlobalMessageChannel<string> _channel;
        private readonly ILogger<EngineState> _logger;
        private readonly OrbitEngine _engine;

        public EngineState(IPluginProvider pluginProvider, 
            OrbitEngine engine, 
            GlobalMessageChannel<string> channel,
            ILogger<EngineState> logger
            )
        {
            _engine = engine;
            _channel = channel;
            _logger = logger;

            ListenToPluginEvents();
        }

        public List<Type> ActivePlugins { get; internal set; } = new List<Type>();
        public OrbitEngine? Engine { get => _engine; }

        //private void HandlePluginEvent(PluginEvent pluginEvent)
        //{
        //    Type? targetPlugin = Engine.Configuration.Plugins.Where(x => x.ContainsPlugins)
        //        .FirstOrDefault(x => x.PluginType.Name == pluginEvent.PluginName)?.PluginType;

        //    if (targetPlugin == null)
        //    {
        //        _engine.LogWarning("Plugin with name {PluginName} not found in configuration.", pluginEvent.PluginName);
        //        return;
        //    }

        //    switch (pluginEvent.Type)
        //    {
        //        case PluginEventType.Activated:
        //            if (!ActivePlugins.Contains(targetPlugin))
        //            {
        //                ActivePlugins.Add(targetPlugin);
        //            }
        //            break;

        //        case PluginEventType.Deactivated:
        //            ActivePlugins.Remove(targetPlugin);
        //            break;
        //    }
        //}

        private void HandlePluginEvent(string json)
        {
            _logger.LogInformation("UpdatedState: {Json}", json);
        }


        private void ListenToPluginEvents()
        {
            _ = Task.Run(async () =>
            {
                await foreach (var pluginEvent in _channel.ReadAllAsync())
                {
                    HandlePluginEvent(pluginEvent);
                }
            });
        }
    }
}