using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Loaders.Plugin;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ORBIT9000.Engine.Providers
{
    internal class PluginProvider : IPluginProvider
    {
        private readonly InitializedInternalConfig _config;
        private readonly ILogger<PluginProvider> _logger;
        private readonly IPluginLoader _pluginLoader;
        private readonly IServiceProvider _provider;

        public PluginProvider(IPluginLoader pluginLoader, ILogger<PluginProvider> logger, InitializedInternalConfig config, IServiceProvider provider)
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

        public AssemblyLoadResult[] GetPluginRegistrationInfo()
        {
            return this._config.PluginInfo;
        }

        public IOrbitPlugin Register(Type plugin)
        {
            IServiceScope scope = _provider.CreateScope();
            return (IOrbitPlugin)RuntimeHelpers.GetUninitializedObject(plugin);
        }
    }
}
