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
        private readonly InitializedInternalConfig _config;
        private readonly ILogger<PluginProvider> _logger;
        private readonly IPluginLoader _pluginLoader;
        private readonly IServiceProvider _provider;

        public PluginProvider(ILogger<PluginProvider> logger, IPluginLoader pluginLoader, InitializedInternalConfig config, IServiceProvider provider)
        {
            _pluginLoader = pluginLoader;
            _logger = logger;
            _config = config;
            _provider = provider;
        }

        public IOrbitPlugin Activate(Type plugin)
        {
            IServiceScope scope = _provider.CreateScope();
            return ActivatorUtilities.CreateInstance(scope.ServiceProvider, plugin, []) as IOrbitPlugin;
        }

        public Type[] GetPluginRegistrationInfo()
        {
            return _config.Plugins.Select(x => x.PluginType).ToArray();
        }

        public IOrbitPlugin Register(Type plugin)
        {
            IServiceScope scope = _provider.CreateScope();
            return (IOrbitPlugin)RuntimeHelpers.GetUninitializedObject(plugin);
        }
    }
}
