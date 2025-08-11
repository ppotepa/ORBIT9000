using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Environment;
using ORBIT9000.Engine.Configuration.Raw;
<<<<<<< HEAD
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
=======
using ORBIT9000.Engine.IO.Loaders.Plugin;
>>>>>>> bfa6c2d (Try fix pipeline)
using ORBIT9000.Engine.IO.Loaders.Plugin.Strategies;

namespace ORBIT9000.Engine.IO.Loaders
{
    internal class PluginLoaderFactory(IServiceProvider serviceProvider, RawEngineConfiguration config)
    {
        private readonly RawEngineConfiguration _rawConfig = config;
        private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public IPluginLoader Create()
        {
<<<<<<< HEAD
<<<<<<< HEAD
            if (_rawConfig == null)
                throw new ArgumentNullException(nameof(_rawConfig));
>>>>>>> 6edfcca (refactor: replace Twitter plugin with Example plugin)

=======
            ArgumentNullException.ThrowIfNull(_rawConfig);
>>>>>>> 86e317a (Refactor interfaces and improve null safety)
=======
            ArgumentNullException.ThrowIfNull(this._rawConfig);
>>>>>>> bfa6c2d (Try fix pipeline)

            return this._rawConfig.Plugins.ActivePlugins.Length switch
            {
                > 0 => this._serviceProvider.GetRequiredService<StringArrayPluginLoader>(),
                _ when AppEnvironment.IsDebug => this._serviceProvider.GetRequiredService<DebugDirectoryPluginLoader>(),
                _ => this._serviceProvider.GetRequiredService<DirectoryPluginLoader>()
            };
        }
    }
}
