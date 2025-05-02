using MessagePack;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Abstractions.Runtime;
using ORBIT9000.Core.Events;
using ORBIT9000.Engine.Configuration;
using System.Linq;

namespace ORBIT9000.Engine.Runtime.State
{
    [MessagePackObject]
    public class EngineState
    {
        private readonly GlobalMessageChannel<PluginEvent> _channel;
        private readonly object _channelLock = new object();
        private readonly OrbitEngine _engine;
        private readonly ILogger<EngineState> _logger;

        public EngineState()
        {
        }

        public EngineState(IPluginProvider pluginProvider,
            OrbitEngine engine,
            GlobalMessageChannel<PluginEvent> channel,
            ILogger<EngineState> logger
            )
        {
            _engine = engine;
            _channel = channel;
            _logger = logger;

            ListenToPluginEvents();
        }

        [Key(0)]
        public List<PluginInfo> ActivatedPlugins { get; internal set; } = [];

        [IgnoreMember]
        public OrbitEngine? Engine { get => _engine; }

        private void HandlePluginEvent(PluginEvent pluginEvent)
        {

            if (Engine?.Configuration?.Plugins == null)
            {
                _engine.LogWarning("Engine configuration or plugins are not properly initialized.");
                return;
            }

            var info = Engine.Configuration.Plugins
                .Where(x => x.ContainsPlugins)
                .FirstOrDefault(x => x.PluginType == pluginEvent.PluginType);

            if (info == null)
            {
                _engine.LogWarning("Plugin with name {PluginName} not found in configuration.", pluginEvent.PluginType);
                return;
            }

            switch (pluginEvent.Type)
            {
                case PluginEventType.Activated:
                    if (!ActivatedPlugins.Contains(info))
                    {
                        ActivatedPlugins.Add(info);
                    }
                    break;

                case PluginEventType.Deactivated:
                    ActivatedPlugins.Remove(info);
                    break;
            }
        }
        private void ListenToPluginEvents()
        {
            _ = Task.Run(async () =>
            {
                _logger.LogInformation("Listening to plugin events...");
                while (this.Engine?.IsRunning == true)
                {
                    IAsyncEnumerable<PluginEvent> events;

                    lock (_channelLock)
                    {
                        events = _channel.ReadAllAsync();
                    }

                    try
                    {
                        await foreach (PluginEvent pluginEvent in events.WithCancellation(CancellationToken.None))
                        {
                            HandlePluginEvent(pluginEvent);
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogWarning(ex, "Plugin event reading was canceled.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while processing plugin events.");
                    }

                    await Task.Delay(1000);
                }
            });
        }
    }
}