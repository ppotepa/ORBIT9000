using Microsoft.Extensions.Logging;
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
using ORBIT9000.Abstractions;
=======
using ORBIT9000.Core.Abstractions.Loaders;
>>>>>>> ed8e1ec (Remove PreBuild Helper)
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;
=======
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine.IO.Loaders.Plugin.Validation;
using ORBIT9000.Engine.Loaders.Plugin.Results;
>>>>>>> e2b2b5a (Reworked Naming)
=======
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;
>>>>>>> 254394d (Remove OverLogging)

namespace ORBIT9000.Engine.IO.Loaders.Plugin
{
    internal abstract class PluginLoaderBase<TSource> : IPluginLoader<TSource>
        where TSource : class
    {
        protected readonly ILogger _logger;
        protected bool _abortOnError = false;
        private readonly IAssemblyLoader _assemblyLoader;
<<<<<<< HEAD

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
=======
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
  
<<<<<<< HEAD
        public abstract IEnumerable<AssemblyLoadResult> LoadPlugins(TSource source);
        public IEnumerable<AssemblyLoadResult> LoadPlugins(object source)
>>>>>>> e2b2b5a (Reworked Naming)
=======
        public abstract IEnumerable<PluginInfo> LoadPlugins(TSource source);

        public IEnumerable<PluginInfo> LoadPlugins(object source)
>>>>>>> 254394d (Remove OverLogging)
        {
            return LoadPlugins((TSource)source);
        }

<<<<<<< HEAD
<<<<<<< HEAD
        protected PluginInfo LoadSingle(string path)
        {
            FileInfo fileInfo = new(path);
=======
        protected AssemblyLoadResult LoadSingle(string path)
=======
        protected PluginInfo LoadSingle(string path)
>>>>>>> 254394d (Remove OverLogging)
        {
            FileInfo fileInfo = new FileInfo(path);
>>>>>>> e2b2b5a (Reworked Naming)

            using (_logger.BeginScope($"{fileInfo.Name}"))
            {
                _logger.LogInformation("Loading Assembly from {Path}", fileInfo.Name);

<<<<<<< HEAD
<<<<<<< HEAD
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
=======
                AssemblyLoadResult details = TryLoadSingleFile(fileInfo);
=======
                PluginInfo result = TryLoadSingleFile(fileInfo);
>>>>>>> 254394d (Remove OverLogging)

                return result;
            }
        }

        private PluginInfo TryLoadSingleFile(FileInfo info)
        {
            var assemblyLoadResult = _assemblyLoader.Load(info);
            
            if(assemblyLoadResult is null)
            {
                return new PluginInfo
                {
                    Assembly = null,
                    FileInfo = info,                    
                };  
            }

            return new PluginInfo
            {
<<<<<<< HEAD
                _logger.LogCritical("Aborting due to errors:");

                foreach (Exception ex in exceptions)
                {
                    _logger.LogCritical(ex, ex.Message);
                }

                if (exceptions.Count is 1)
                {
                    throw exceptions[0];
                }
                if (exceptions.Count > 1)
                {
                    throw new AggregateException("Multiple errors occurred while loading plugins.", exceptions);
                }
            }
        }

        private AssemblyLoadResult TryLoadSingleFile(FileInfo info)
        {
            var fileValidator = new PluginFileValidator(info, _logger);
            var validationExceptions = fileValidator.Validate().Exceptions;

            if (!fileValidator.IsValid)
            {
                return CreateAssemblyLoadResult(fileValidator, null, validationExceptions);
            }

            var assemblyLoadResult = _assemblyLoader.TryLoadAssembly(info);
            var allExceptions = validationExceptions
                .Concat(assemblyLoadResult.Exceptions ?? Enumerable.Empty<Exception>())
                .ToList();

            if (assemblyLoadResult.PluginTypes.Length == 0)
            {
                _logger.LogInformation("No plugins found in {Assembly}. Unloading ...", info.FullName);
                _assemblyLoader.UnloadAssembly(info);
            }

            if (allExceptions.Any())
            {
                HandleErrors(allExceptions);
            }

            return CreateAssemblyLoadResult(fileValidator, assemblyLoadResult, allExceptions);
>>>>>>> e2b2b5a (Reworked Naming)
=======
                Assembly = assemblyLoadResult,
                FileInfo = info,
                PluginType = assemblyLoadResult.GetTypes()
                    .FirstOrDefault(type => type.IsClass && type.GetInterfaces().Contains(typeof(IOrbitPlugin))),               
            };
>>>>>>> 254394d (Remove OverLogging)
        }
    }
}
