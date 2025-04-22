using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Core.Extensions.IO.Files;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class DebugDirectoryPluginLoader : PluginLoaderBase<DirectoryInfo>
    {
        private static readonly string[] SKIP_FOLDERS = { "obj", "ref", "Release" };

        public DebugDirectoryPluginLoader(ILogger<DebugDirectoryPluginLoader>? logger, Configuration.Raw.RawConfiguration config, IAssemblyLoader loader) 
            : base(logger, config, loader)
        {
        }

        public override IEnumerable<AssemblyLoadResult> LoadPlugins(DirectoryInfo source)
        {
            source = FindSrcFolder(source);

            if (source.EnumerateDirectories() is var subdirs && subdirs.Any(info => info.Name == "Plugins"))
            {
                this._logger.LogWarning("Source directory {Name} contains subdirectories. " +
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
        }
    }
}