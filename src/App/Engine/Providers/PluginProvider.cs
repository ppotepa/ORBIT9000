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
        // Stores single-instance plugins
        private readonly Dictionary<Type, IOrbitPlugin> _activePlugins = new();

        private readonly RuntimeConfiguration _config;
        private readonly ILogger<PluginProvider> _logger;
        private readonly IPluginLoader _pluginLoader;
        private readonly ILifetimeScope _rootScope;
        private readonly List<PluginInfo> _validPlugins;

        public PluginProvider(
            ILogger<PluginProvider> logger,
            RuntimeConfiguration config,
            IPluginLoader pluginLoader,
            ILifetimeScope rootScope
        )
        {
            _logger = logger;
            _config = config;
            _pluginLoader = pluginLoader;
            _rootScope = rootScope;
            _validPlugins = _config.Plugins
                .Where(x => x.ContainsPlugins)
                .ToList();
        }

        public IOrbitPlugin Activate(object plugin)
        {
            if (plugin is string pluginName)
            {
                PluginInfo? target = _validPlugins.FirstOrDefault(x => x.PluginType.Name.Contains(pluginName));
                if (target != null)
                {
                    if (target.IsSingleton && _activePlugins.TryGetValue(target.PluginType, out var existingInstance))
                    {
                        _logger.LogInformation("Plugin already active (singleton): {Plugin}", pluginName);
                        return existingInstance;
                    }

                    var scope = _rootScope.BeginLifetimeScope(builder =>
                    {
                        ServiceCollection services = new ServiceCollection();
                        IOrbitPlugin dummy = (IOrbitPlugin)RuntimeHelpers.GetUninitializedObject(target.PluginType);
                        dummy.RegisterServices(services);
                        builder.Populate(services);
                    });

                    var instance = (IOrbitPlugin)CreateInstanceFromScope(target.PluginType, scope);

                    if (target.IsSingleton)
                    {
                        _activePlugins[target.PluginType] = instance;
                    }

                    _logger.LogInformation("Plugin activated: {Plugin}", pluginName);
                    return instance;
                }
            }

            _logger.LogError("Plugin activation failed. Invalid plugin identifier: {Plugin}", plugin);
            throw new ArgumentException("Invalid plugin identifier.", nameof(plugin));
        }

        public IOrbitPlugin Activate(Type plugin)
        {
            throw new NotImplementedException();
        }

        public void Unload(object plugin)
        {
            _pluginLoader.Unload(plugin);
        }

        private static object CreateInstanceFromScope(Type type, ILifetimeScope scope)
        {
            var serviceProvider = scope.Resolve<IServiceProvider>();
            return ActivatorUtilities.CreateInstance(serviceProvider, type);
        }
    }
}