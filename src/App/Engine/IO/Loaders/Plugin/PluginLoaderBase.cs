using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
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

            this._logger = logger;
            this._assemblyLoader = assemblyLoader;

            this._logger.LogDebug("PluginLoader constructor called. Type invoked {Type}", this.GetType());
        }

        public abstract IEnumerable<PluginInfo> LoadPlugins(TSource source);

        public IEnumerable<PluginInfo> LoadPlugins(object source)
        {
            return this.LoadPlugins((TSource)source);
        }

        protected PluginInfo LoadSingle(string path)
        {
            FileInfo fileInfo = new(path);

            using (this._logger.BeginScope($"{fileInfo.Name}"))
            {
                this._logger.LogInformation("Loading Assembly from {Path}", fileInfo.Name);

                PluginInfo result = this.TryLoadSingleFile(fileInfo);

                return result;
            }
        }

        private PluginInfo TryLoadSingleFile(FileInfo info)
        {
            System.Reflection.Assembly? assemblyLoadResult = this._assemblyLoader.Load(info);

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
                this._logger.LogWarning("No plugin type found in assembly {Assembly}", info.Name);
            }

            return new PluginInfo
            {
                Assembly = assemblyLoadResult,
                FileInfo = info,
                PluginType = pluginType is not null ? pluginType : typeof(VoidType),
            };
        }

        public void Unload(object plugin)
        {
            throw new NotImplementedException();
        }
    }
}
