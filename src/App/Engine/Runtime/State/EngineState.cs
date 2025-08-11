<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
﻿using MessagePack;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions.Runtime;
using ORBIT9000.Core.Events;
using ORBIT9000.Engine.Configuration;

namespace ORBIT9000.Engine.Runtime.State
{
    [MessagePackObject]
    public class EngineState
    {
        private readonly GlobalMessageChannel<PluginEvent> _channel;
        private readonly object _channelLock = new();
        private readonly ILogger<EngineState> _logger;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public EngineState()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
        }

        public EngineState(OrbitEngine engine,
            GlobalMessageChannel<PluginEvent> channel,
            ILogger<EngineState> logger
            )
        {
            Engine = engine;
            _channel = channel;
            _logger = logger;

            ListenToPluginEvents();
        }

        [Key(0)]
        public List<PluginInfo> ActivatedPlugins { get; internal set; } = [];

        [IgnoreMember]
        public OrbitEngine? Engine { get; }

        private void HandlePluginEvent(PluginEvent pluginEvent)
        {
            if (Engine?.Configuration?.Plugins == null)
            {
                Engine?.LogWarning("Engine configuration or plugins are not properly initialized.");
                return;
            }

            PluginInfo? info = Engine.Configuration.Plugins
                .Where(x => x.ContainsPlugins)
                .FirstOrDefault(x => x.PluginType == pluginEvent.PluginType);

            if (info == null)
            {
                Engine.LogWarning("Plugin with name {PluginName} not found in configuration.", pluginEvent.PluginType);
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
                while (Engine?.IsRunning == true)
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
=======
﻿namespace ORBIT9000.Engine.Runtime.State
{
    public class EngineState
    {
        public OrbitEngine? Engine { get; internal set; }
>>>>>>> e2b2b5a (Reworked Naming)
=======
﻿using ORBIT9000.Abstractions;
using ORBIT9000.Core.Events;
=======
﻿using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Abstractions.Runtime;
>>>>>>> 56ba6c0 (Add Generic Message Channel)

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
>>>>>>> 2e9d040 (Add Basic Plugin Channel Handling)
    }
}