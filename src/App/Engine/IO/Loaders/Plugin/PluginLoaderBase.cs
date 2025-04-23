using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin
{
    internal abstract class PluginLoaderBase<TSource> : IPluginLoader<TSource>
        where TSource : class
    {
        protected readonly ILogger _logger;
        protected bool _abortOnError = false;
        private readonly IAssemblyLoader _assemblyLoader;
        private readonly Configuration.Raw.RawConfiguration _config;

        protected PluginLoaderBase(ILogger? logger, Configuration.Raw.RawConfiguration config, IAssemblyLoader assemblyLoader)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(assemblyLoader);

            _logger = logger;
            _config = config;
            _assemblyLoader = assemblyLoader;

            _logger.LogDebug("PluginLoader constructor called. Type invoked {Type}", this.GetType());
        }
  
        public abstract IEnumerable<PluginInfo> LoadPlugins(TSource source);

        public IEnumerable<PluginInfo> LoadPlugins(object source)
        {
            return ((IPluginLoader<TSource>)this).LoadPlugins((TSource)source);
        }

        protected PluginInfo LoadSingle(string path)
        {
            FileInfo fileInfo = new FileInfo(path);

            using (_logger.BeginScope($"{fileInfo.Name}"))
            {
                _logger.LogInformation("Loading Assembly from {Path}", fileInfo.Name);

                PluginInfo result = TryLoadSingleFile(fileInfo);

                return result;
            }
        }

        private PluginInfo TryLoadSingleFile(FileInfo info)
        {
            var assemblyLoadResult = _assemblyLoader.Load(info);
            return new PluginInfo
            {
                Assembly = assemblyLoadResult,
                FileInfo = info
            };
        }
    }
}
