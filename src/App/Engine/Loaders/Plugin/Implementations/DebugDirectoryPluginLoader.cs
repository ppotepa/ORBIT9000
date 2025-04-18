using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Extensions.IO.Files;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.Loaders.Plugin.Implementations
{
    internal class DebugDirectoryPluginLoader : PluginLoaderBase<DirectoryInfo>
    {
        private static readonly string[] SKIP_FOLDERS = { "obj", "ref" };

        public DebugDirectoryPluginLoader(ILogger? logger) : base(logger)
        {
        }

        public override IEnumerable<PluginLoadResult> LoadPlugins(DirectoryInfo source)
        {
            if (!source.Exists)
            {
                this._logger?.LogWarning($"Source directory {source.FullName} does not exist.");
                yield break;
            }

            if (source.EnumerateDirectories() is var subdirs && subdirs.Any(info => info.Name == "Plugins"))
            {
                this._logger?.LogWarning($"Source directory {source.FullName} contains subdirectories. " +
                    $"Only the top-level directory will be used.");

                DirectoryInfo newSource = new DirectoryInfo(Path.Combine(source.FullName, "Plugins"));
                FileInfo[] files = [.. newSource.GetFilesExcept("*.dll", SKIP_FOLDERS)];
                  
                if (files.Length == 0)
                {
                    this._logger?.LogWarning($"No plugins found in {newSource.FullName}");
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
    }
}