using Microsoft.Extensions.Logging;
<<<<<<< HEAD
<<<<<<< HEAD
using ORBIT9000.Core.Extensions.IO;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class DebugDirectoryPluginLoader(ILogger<DebugDirectoryPluginLoader> logger, IAssemblyLoader loader)
        : PluginLoaderBase<DirectoryInfo>(logger, loader)
    {
        private static readonly string[] SkipFolders = ["obj", "ref", "Release"];

        public override IEnumerable<PluginInfo> LoadPlugins(DirectoryInfo source)
        {
            ValidateSourceArgument(source);

            source = FindSrcFolder(source)!;

            if (source is null)
            {
                _logger.LogWarning("No 'src' folder found starting from {Name}", source?.FullName ?? "unknown");
                yield break;
            }

            IEnumerable<DirectoryInfo> subdirs = source.EnumerateDirectories();
            if (!subdirs.Any(d => d.Name.Equals("Plugins", StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("No 'Plugins' directory found under {Name}", source.FullName);
                yield break;
            }

            DirectoryInfo pluginDir = new(Path.Combine(source.FullName, "Plugins"));
            IEnumerable<FileInfo> files = [.. pluginDir.GetFilesExcept("*.odll", SkipFolders)];

            if (!files.Any())
            {
                _logger.LogWarning("No plugins found in {Name}", pluginDir.FullName);
                yield break;
            }

            foreach (FileInfo file in files)
            {
                yield return LoadSingle(file.FullName);
            }
        }

        private static void ValidateSourceArgument(DirectoryInfo source)
        {
            ArgumentNullException.ThrowIfNull(source);
        }

        private static DirectoryInfo? FindSrcFolder(DirectoryInfo? currentDirectory)
        {
            while (currentDirectory is not null)
            {
                if (currentDirectory.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
                    return currentDirectory;

                currentDirectory = currentDirectory.Parent;
            }

            return null;
=======
using ORBIT9000.Core.Abstractions.Loaders;
=======
>>>>>>> 254394d (Remove OverLogging)
using ORBIT9000.Core.Extensions.IO.Files;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class DebugDirectoryPluginLoader : PluginLoaderBase<DirectoryInfo>
    {
        private static readonly string[] SKIP_FOLDERS = { "obj", "ref", "Release" };

        public DebugDirectoryPluginLoader(ILogger<DebugDirectoryPluginLoader>? logger, Configuration.Raw.RawEngineConfiguration config, IAssemblyLoader loader) 
            : base(logger, config, loader)
        {
        }

        public override IEnumerable<PluginInfo> LoadPlugins(DirectoryInfo source)
        {
            source = FindSrcFolder(source);

            if (source.EnumerateDirectories() is var subdirs && subdirs.Any(info => info.Name == "Plugins"))
            {
                this._logger.LogInformation("Source directory {Name} contains subdirectories. " +
                    $"Only the top-level directory will be used.", source.FullName);

                DirectoryInfo newSource = new DirectoryInfo(Path.Combine(source.FullName, "Plugins"));
                FileInfo[] files = [.. newSource.GetFilesExcept("*.dll", SKIP_FOLDERS)];
                  
                if (files.Length == 0)
                {
                    this._logger.LogWarning("No plugins found in {Name}", newSource.FullName);
                }
                else
                {
                    foreach (FileInfo file in files)
                    {
                        yield return LoadSingle(file.FullName);
                    }
                }
            }
        }

        private DirectoryInfo? FindSrcFolder(DirectoryInfo currentDirectory)
        {
            if (currentDirectory == null || !currentDirectory.Exists)
            {
                return null;
            }

            if (currentDirectory.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
            {
                return currentDirectory;
            }

            return FindSrcFolder(currentDirectory.Parent!);
>>>>>>> e2b2b5a (Reworked Naming)
        }
    }
}