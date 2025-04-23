using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Abstractions.Loaders;
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
        private readonly ServiceProvider _pluginServiceProvider;
        private readonly IServiceProvider _provider;
        private readonly List<PluginInfo> _validPlugins;

        public PluginProvider(ILogger<PluginProvider> logger, 
            IPluginLoader pluginLoader, 
            RuntimeConfiguration config, 
            IServiceProvider provider,
            IServiceCollection rootCollection
            )
        {
            _pluginLoader = pluginLoader;
            _logger = logger;
            _config = config;
            _provider = provider;
            _validPlugins = _config.Plugins.Where(x => x.ContainsPlugins).ToList();

            this._validPlugins.ForEach(plugin =>
            {
                var dummy = (IOrbitPlugin)RuntimeHelpers.GetUninitializedObject(plugin.PluginType);
                dummy.RegisterServices(rootCollection);
            });

            _pluginServiceProvider = rootCollection.BuildServiceProvider();
        }

        public IOrbitPlugin Activate(object plugin)
        {
            if (plugin is string pluginName)
            {
                var target = _validPlugins.FirstOrDefault(x => x.PluginType.Name.Contains(pluginName));
                if (target != null)
                {
                    var scope = _pluginServiceProvider.CreateScope();
                    IOrbitPlugin? instance = (IOrbitPlugin)ActivatorUtilities.CreateInstance(scope.ServiceProvider, target.PluginType);
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
    }
}
