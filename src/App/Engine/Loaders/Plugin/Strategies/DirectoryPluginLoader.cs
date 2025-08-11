using Microsoft.Extensions.Logging;
<<<<<<< HEAD
<<<<<<< HEAD
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;
=======
=======
using ORBIT9000.Core.Abstractions.Loaders;
>>>>>>> a1c6c63 (Refactor plugin architecture and configuration handling)
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders.Plugin.Results;
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)

<<<<<<<< HEAD:src/App/Engine/IO/Loaders/Plugin/Strategies/DirectoryPluginLoader.cs
namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
========
namespace ORBIT9000.Engine.Loaders.Plugin.Strategies
>>>>>>>> 7611f11 (Refactor plugin loading and configuration handling):src/App/Engine/Loaders/Plugin/Strategies/DirectoryPluginLoader.cs
{
    internal class DirectoryPluginLoader(ILogger<DirectoryPluginLoader> logger, IAssemblyLoader loader)
        : PluginLoaderBase<DirectoryInfo>(logger, loader)
    {
<<<<<<< HEAD
<<<<<<< HEAD
        public override IEnumerable<PluginInfo> LoadPlugins(DirectoryInfo source)
=======
        public DirectoryPluginLoader(ILogger? logger, OrbitEngineConfiguration config) : base(logger, config)
        {
        }

        public override IEnumerable<PluginLoadResult> LoadPlugins(DirectoryInfo source)
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
=======
        public DirectoryPluginLoader(ILogger? logger, Configuration.Raw.RawConfiguration config, IAssemblyLoader loader) : base(logger, config, loader)
        {
        }

        public override IEnumerable<Results.AssemblyLoadResult> LoadPlugins(DirectoryInfo source)
>>>>>>> a1c6c63 (Refactor plugin architecture and configuration handling)
        {
            if (!source.Exists)
                source.Create();

            FileInfo[] files = source.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
<<<<<<< HEAD
                _logger.LogWarning("No plugins found in {Name}", source.FullName);
=======
                this._logger?.LogWarning($"No plugins found in {source.FullName}");
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
            }
            else
            {
                foreach (FileInfo file in source.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
                {
                    yield return LoadSingle(file.FullName);
                }
            }
        }
    }
}