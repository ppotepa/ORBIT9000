using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Events;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace ORBIT9000.Engine.Providers
{
    internal class PluginProvider : ORBIT9000.Core.Environment.Disposable, IPluginProvider
    {
        private readonly Dictionary<Type, IOrbitPlugin> _activePlugins = new();
        private readonly RuntimeConfiguration _config;
        private readonly ILogger<PluginProvider> _logger;
        private readonly Channel<PluginEvent> _pluginEvents = Channel.CreateUnbounded<PluginEvent>();
        private readonly IPluginLoader _pluginLoader;
        private readonly ILifetimeScope _rootScope;
        private readonly List<PluginInfo> _validPlugins;
        private Dictionary<Type, ILifetimeScope>? _individualPluginScopes;
        private ILifetimeScope? _pluginScope;
        public PluginProvider
        (
            ILogger<PluginProvider> logger,
            RuntimeConfiguration config,
            IPluginLoader pluginLoader,
            ILifetimeScope rootScope)
        {
            _logger = logger;
            _config = config;
            _pluginLoader = pluginLoader;
            _rootScope = rootScope;
            _validPlugins = _config.Plugins.Where(x => x.ContainsPlugins).ToList();
        }

        public ChannelReader<PluginEvent> PluginEvents => _pluginEvents.Reader;
        private Dictionary<Type, ILifetimeScope> IndividualPluginScopes
        {
            get
            {
                if (_individualPluginScopes == null)
                {
                    _individualPluginScopes = new Dictionary<Type, ILifetimeScope>();
                    foreach (var info in _validPlugins)
                    {
                        _individualPluginScopes[info.PluginType] = _rootScope.BeginLifetimeScope(builder =>
                        {
                            ServiceCollection services = new ServiceCollection();
                            RegisterPlugin(builder, services, info);
                            builder.Populate(services);
                        });
                    }
                }
                return _individualPluginScopes;
            }
        }

        private ILifetimeScope PluginScope => _pluginScope ??= CreateSharedScope();
        public async Task<IOrbitPlugin> Activate(object plugin)
        {
            if (plugin is string pluginName)
            {
                var target = _validPlugins.FirstOrDefault(x => x.PluginType.Name.Contains(pluginName));
                if (target != null)
                {
                    return await ActivatePlugin(target);
                }
            }

            _logger.LogError("Plugin activation failed. Invalid plugin identifier: {Plugin}", plugin);
            throw new ArgumentException("Invalid plugin identifier.", nameof(plugin));
        }

        public Task<IOrbitPlugin> Activate(Type plugin)
        {
            throw new NotImplementedException();
        }

        public void Unload(object plugin)
        {
            _pluginLoader.Unload(plugin);
        }

        protected override void DisposeManagedObjects()
        {
            if (_pluginScope != null)
            {
                _pluginScope.Dispose();
                _pluginScope = null;
            }

            if (_individualPluginScopes != null)
            {
                foreach (var scope in _individualPluginScopes.Values)
                {
                    scope.Dispose();
                }

                _individualPluginScopes.Clear();
                _individualPluginScopes = null;
            }
        }

        private static void RegisterPlugin(ContainerBuilder builder, ServiceCollection services, PluginInfo info)
        {
            if (info.IsSingleton)
            {
                builder.RegisterType(info.PluginType).AsSelf().As<IOrbitPlugin>().SingleInstance();
            }
            else
            {
                builder.RegisterType(info.PluginType).AsSelf().As<IOrbitPlugin>().InstancePerDependency();
            }

            IOrbitPlugin dummy = (IOrbitPlugin)RuntimeHelpers.GetUninitializedObject(info.PluginType);
            dummy.RegisterServices(services);
        }

        private async Task<IOrbitPlugin> ActivatePlugin(PluginInfo target)
        {
            if (target.IsSingleton && _activePlugins.TryGetValue(target.PluginType, out var existingInstance))
            {
                _logger.LogInformation("Plugin already active (singleton): {Plugin}", target.PluginType.Name);
                return existingInstance;
            }

            var instance = CreateInstanceFromScope(target.PluginType);

            if (instance != null && target.IsSingleton)
            {
                _activePlugins[target.PluginType] = instance;
            }

            _logger.LogInformation("Plugin activated: {Plugin}", target.PluginType.Name);

            await _pluginEvents.Writer.WriteAsync(new PluginEvent
            {
                Type = PluginEventType.Activated,
                PluginName = target.PluginType.Name
            });

            return instance!;
        }

        private IOrbitPlugin? CreateInstanceFromScope(Type type)
        {
            try
            {
                IServiceProvider serviceProvider = _config.SharePluginScopes
                    ? PluginScope.Resolve<IServiceProvider>()
                    : IndividualPluginScopes[type].Resolve<IServiceProvider>();

                return (IOrbitPlugin?)ActivatorUtilities.CreateInstance(serviceProvider, type);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Failed to create instance from scope for type {Type}", type);
                return null;
            }
        }

        private ILifetimeScope CreateSharedScope()
        {
            return _rootScope.BeginLifetimeScope(builder =>
            {
                ServiceCollection services = new ServiceCollection();

                foreach (PluginInfo info in _validPlugins)
                {
                    RegisterPlugin(builder, services, info);
                }

                builder.Populate(services);
            });
        }
    }
}
