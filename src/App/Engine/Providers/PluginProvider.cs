using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Core.Abstractions.Providers;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using System.Runtime.CompilerServices;

namespace ORBIT9000.Engine.Providers
{
    internal class PluginProvider : IPluginProvider
    {
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
            _pluginLoader = pluginLoader;
            _logger = logger;
            _config = config;
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
                    // Create a child scope for plugin
                    var scope = _rootScope.BeginLifetimeScope(builder =>
                    {
                        ServiceCollection collection = new ServiceCollection();
                        IOrbitPlugin dummy = (IOrbitPlugin) RuntimeHelpers.GetUninitializedObject(target.PluginType);
                        dummy.RegisterServices(collection);
                        builder.Populate(collection);
                    });
                   
                    return (IOrbitPlugin)CreateInstanceFromScope(target.PluginType, scope);
                }
            }

            _logger.LogError("Plugin activation failed. Invalid plugin identifier: {Plugin}", plugin);
            throw new ArgumentException("Invalid plugin identifier.", nameof(plugin));
        }

        public IOrbitPlugin Activate(Type plugin)
        {
            throw new NotImplementedException();
        }

        public object CreateInstanceFromScope(Type type, ILifetimeScope scope)
        {
            IServiceProvider serviceProvider = scope.Resolve<IServiceProvider>();
            return ActivatorUtilities.CreateInstance(serviceProvider, type);
        }
    }
}