using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Extensions.IO;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class DebugDirectoryPluginLoader(ILogger<DebugDirectoryPluginLoader> logger,
        IAssemblyLoader loader) : PluginLoaderBase<DirectoryInfo>(logger, loader)
    {
        private static readonly string[] SKIP_FOLDERS = ["obj", "ref", "Release"];

        public override IEnumerable<PluginInfo> LoadPlugins(DirectoryInfo source)
        {
            source = FindSrcFolder(source)!;

            if (source.EnumerateDirectories() is var subdirs && subdirs.Any(info => info.Name == "Plugins"))
            {
                this._logger.LogInformation("Source directory {Name} contains subdirectories. " +
                    "Only the top-level directory will be used.", source.FullName);

                DirectoryInfo newSource = new(Path.Combine(source.FullName, "Plugins"));
                FileInfo[] files = [.. newSource.GetFilesExcept("*.dll", SKIP_FOLDERS)];

                if (files.Length == 0)
                {
                    this._logger.LogWarning("No plugins found in {Name}", newSource.FullName);
                }
                else
                {
                    foreach (FileInfo file in files)
                    {
                        yield return this.LoadSingle(file.FullName);
                    }
                }
            }
        }

        private static DirectoryInfo? FindSrcFolder(DirectoryInfo currentDirectory)
        {
            if (currentDirectory?.Exists != true)
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