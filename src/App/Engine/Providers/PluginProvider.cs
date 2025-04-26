using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using System.Runtime.CompilerServices;

namespace ORBIT9000.Engine.Providers
{
    internal class PluginProvider : IPluginProvider
    {
        private readonly Dictionary<Type, IOrbitPlugin> _activePlugins = new();
        private readonly RuntimeConfiguration _config;
        private readonly Dictionary<Type, ILifetimeScope> _individualPluginScopes = new();
        private readonly ILogger<PluginProvider> _logger;
        private readonly IPluginLoader _pluginLoader;
        private readonly ILifetimeScope _rootScope;
        private readonly List<PluginInfo> _validPlugins;
        private ILifetimeScope _pluginScope;

        public PluginProvider(
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

        public IOrbitPlugin Activate(object plugin)
        {
            if (plugin is string pluginName)
            {
                var target = _validPlugins.FirstOrDefault(x => x.PluginType.Name.Contains(pluginName));
                if (target != null)
                {
                    return ActivatePlugin(target);
                }
            }

            _logger.LogError("Plugin activation failed. Invalid plugin identifier: {Plugin}", plugin);
            throw new ArgumentException("Invalid plugin identifier.", nameof(plugin));
        }

        public IOrbitPlugin Activate(Type plugin)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            if (_config.SharePluginScopes)
            {
                _pluginScope = CreateSharedScope();
            }
            else
            {
                CreateIndividualScopes();
            }
        }
        public void Unload(object plugin)
        {
            _pluginLoader.Unload(plugin);
        }

        private IOrbitPlugin ActivatePlugin(PluginInfo target)
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
            return instance!;
        }

        private void CreateIndividualScopes()
        {
            foreach (var info in _validPlugins)
            {
                var pluginScope = _rootScope.BeginLifetimeScope(builder =>
                {
                    var services = new ServiceCollection();
                    RegisterPlugin(builder, services, info);
                    builder.Populate(services);
                });

                _individualPluginScopes[info.PluginType] = pluginScope;
            }
        }

        private IOrbitPlugin? CreateInstanceFromScope(Type type)
        {
            try
            {
                var serviceProvider = _config.SharePluginScopes
                    ? _pluginScope.Resolve<IServiceProvider>()
                    : _individualPluginScopes[type].Resolve<IServiceProvider>();

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
                var services = new ServiceCollection();
                foreach (var info in _validPlugins)
                {
                    RegisterPlugin(builder, services, info);
                }
                builder.Populate(services);
            });
        }

        private void RegisterPlugin(ContainerBuilder builder, ServiceCollection services, PluginInfo info)
        {
            if (info.IsSingleton)
            {
                builder.RegisterType(info.PluginType).AsSelf().As<IOrbitPlugin>().SingleInstance();
            }
            else
            {
                builder.RegisterType(info.PluginType).AsSelf().As<IOrbitPlugin>().InstancePerDependency();
            }

            var dummy = (IOrbitPlugin)RuntimeHelpers.GetUninitializedObject(info.PluginType);
            dummy.RegisterServices(services);
        }
    }
}
