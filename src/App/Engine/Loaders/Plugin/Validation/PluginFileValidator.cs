using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine.Loaders.Plugin.Validation
{
    /// <summary>
    /// Helper class to validate plugin files.
    /// </summary>
    internal class PluginFileValidator
    {
        private readonly ILogger? _logger;
        private readonly string _path;
        public PluginFileValidator(string path, ILogger? logger)
        {
            _path = path;
            _logger = logger;
        }

        public bool FileExists { get; private set; }
        public bool IsDll { get; private set; }
        public bool IsValid => FileExists && IsDll;

        /// <summary>
        /// Validates the plugin file Path and type.
        /// </summary>
        public void Validate(List<Exception> exceptions)
        {
            if (string.IsNullOrWhiteSpace(_path))
            {
                this._logger?.LogWarning($"Path is empty or null: {_path}");
                exceptions.Add(new ArgumentException($"Path is empty or null: {_path}"));
                return;
            }

            FileExists = File.Exists(_path);

            if (!FileExists)
            {
                this._logger?.LogWarning($"File does not exist: {_path}");
                exceptions.Add(new FileNotFoundException($"File does not exist: {_path}"));
                return;
            }

            IsDll = string.Equals(Path.GetExtension(_path), ".dll", StringComparison.OrdinalIgnoreCase);

            if (!IsDll)
            {
                this._logger?.LogWarning($"File is not a DLL: {_path}");
                exceptions.Add(new ArgumentException($"File is not a DLL: {_path}"));
            }
        }
    }
}
