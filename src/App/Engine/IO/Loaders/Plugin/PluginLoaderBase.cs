using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
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

        protected PluginLoaderBase(ILogger logger, IAssemblyLoader assemblyLoader)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(assemblyLoader);

            _logger = logger;
            _assemblyLoader = assemblyLoader;

            _logger.LogDebug("PluginLoader constructor called. Type invoked {Type}", GetType());
        }

        public abstract IEnumerable<PluginInfo> LoadPlugins(TSource source);

        public IEnumerable<PluginInfo> LoadPlugins(object source)
        {
            return LoadPlugins((TSource)source);
        }

        protected PluginInfo LoadSingle(string path)
        {
            FileInfo fileInfo = new(path);

            using (_logger.BeginScope($"{fileInfo.Name}"))
            {
                _logger.LogInformation("Loading Assembly from {Path}", fileInfo.Name);

                PluginInfo result = TryLoadSingleFile(fileInfo);

                return result;
            }
        }

        private PluginInfo TryLoadSingleFile(FileInfo info)
        {
            System.Reflection.Assembly? assemblyLoadResult = _assemblyLoader.Load(info);

            if (assemblyLoadResult is null)
            {
                return new PluginInfo
                {
                    Assembly = null!,
                    FileInfo = info,
                };
            }

            Type? pluginType = assemblyLoadResult.GetTypes()
                .FirstOrDefault(type => type.IsClass && typeof(IOrbitPlugin).IsAssignableFrom(type));

            if (pluginType is null)
            {
                _logger.LogWarning("No plugin type found in assembly {Assembly}", info.Name);
            }

            return new PluginInfo
            {
                Assembly = assemblyLoadResult,
                FileInfo = info,
                PluginType = pluginType ?? typeof(VoidType),
            };
        }

        public void Unload(object plugin)
        {
            throw new NotImplementedException();
        }
    }
}
