using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Environment;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using ORBIT9000.Engine.IO.Loaders.Plugin.Strategies;

namespace ORBIT9000.Engine.IO.Loaders
{
    internal class PluginLoaderFactory(IServiceProvider serviceProvider, RawEngineConfiguration config)
    {
        private readonly RawEngineConfiguration _rawConfig = config;
        private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public IPluginLoader Create()
        {
            ArgumentNullException.ThrowIfNull(this._rawConfig);

            return this._rawConfig.Plugins.ActivePlugins.Length switch
            {
                > 0 => this._serviceProvider.GetRequiredService<StringArrayPluginLoader>(),
                _ when AppEnvironment.IsDebug => this._serviceProvider.GetRequiredService<DebugDirectoryPluginLoader>(),
                _ => this._serviceProvider.GetRequiredService<DirectoryPluginLoader>()
            };
        }
    }
}
