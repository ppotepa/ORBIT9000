using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Environment;
using ORBIT9000.Engine.Configuration.Raw;
<<<<<<< HEAD
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
            ArgumentNullException.ThrowIfNull(_rawConfig);
=======
using ORBIT9000.Engine.IO.Loaders.Plugin.Strategies;

namespace ORBIT9000.Engine.IO.Loaders.Plugin
{
    internal class PluginLoaderFactory
    {
        private readonly RawEngineConfiguration _rawConfig;
        private readonly IServiceProvider _serviceProvider;
        public PluginLoaderFactory(IServiceProvider serviceProvider, RawEngineConfiguration config)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _rawConfig = config;
        }

        public IPluginLoader Create()
        {
            if (_rawConfig == null)
                throw new ArgumentNullException(nameof(_rawConfig));
>>>>>>> 6edfcca (refactor: replace Twitter plugin with Example plugin)


            return _rawConfig.Plugins.ActivePlugins.Length switch
            {
                > 0 => _serviceProvider.GetRequiredService<StringArrayPluginLoader>(),
                _ when AppEnvironment.IsDebug => _serviceProvider.GetRequiredService<DebugDirectoryPluginLoader>(),
                _ => _serviceProvider.GetRequiredService<DirectoryPluginLoader>()
            };
        }
    }
}
