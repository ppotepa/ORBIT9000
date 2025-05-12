using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Extensions.IO;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{

    internal class DebugDirectoryPluginLoader(ILogger<DebugDirectoryPluginLoader> logger, IAssemblyLoader loader
    ) : PluginLoaderBase<DirectoryInfo>(logger, loader)
    {
        private static readonly string[] SkipFolders = ["obj", "ref", "Release"];
        private static readonly string[] SkipFilePatterns =
        [
                "Microsoft.",
                "System.",
                "netstandard",
                "Serilog.",
                "Newtonsoft.",
                "runtime.",
                "Accessibility",
                "WindowsBase",
                "PresentationCore",
                "PresentationFramework",
                "mscorlib",
                "ORBIT9000.Core",
                "ORBIT9000.Engine",
                "ORBIT9000.Data",
        ];

        public override IEnumerable<PluginInfo> LoadPlugins(DirectoryInfo source)
        {
            ArgumentNullException.ThrowIfNull(source);

            source = FindSrcFolder(source)!;

            if (source is null)
            {
                this._logger.LogWarning("No 'src' folder found starting from {Name}", source?.FullName ?? "unknown");
                yield break;
            }

            IEnumerable<DirectoryInfo> subdirs = source.EnumerateDirectories();
            if (!subdirs.Any(d => d.Name.Equals("Plugins", StringComparison.OrdinalIgnoreCase)))
            {
                this._logger.LogWarning("No 'Plugins' directory found under {Name}", source.FullName);
                yield break;
            }

            DirectoryInfo pluginDir = new(Path.Combine(source.FullName, "Plugins"));
            IEnumerable<FileInfo> files = [.. pluginDir.GetFilesExcept("*.dll", SkipFolders, SkipFilePatterns)];

            if (!files.Any())
            {
                this._logger.LogWarning("No plugins found in {Name}", pluginDir.FullName);
                yield break;
            }

            foreach (FileInfo file in files)
            {
                yield return this.LoadSingle(file.FullName);
            }
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
        }
    }
}
