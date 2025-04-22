using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Validation
{
    /// <summary>
    /// Helper class to validate plugin files.
    /// </summary>
    internal class PluginFileValidator
    {
        public List<Exception> Exceptions = new List<Exception>();
        private readonly FileInfo _info;
        private readonly ILogger? _logger;
        public PluginFileValidator(FileInfo info, ILogger? logger)
        {
            _info = info;
            _logger = logger;
        }

        public bool FileExists => _info.Exists;
        public bool IsDll => string.Equals(_info.Extension, ".dll", StringComparison.OrdinalIgnoreCase);
        public bool IsValid => FileExists && IsDll;
        /// <summary>
        /// Validates the plugin file Path and type.
        /// </summary>
        /// <returns>A list of exceptions encountered during validation.</returns>
        public PluginFileValidator Validate()
        {
            if (!FileExists)
            {
                _logger.LogWarning("File does not exist: {FilePath}", _info.FullName);
                Exceptions.Add(new FileNotFoundException($"File does not exist: {_info.FullName}"));
            }

            if (!IsDll)
            {
                _logger.LogWarning("File is not a DLL: {FilePath}", _info.FullName);
                Exceptions.Add(new ArgumentException($"File is not a DLL: {_info.FullName}"));
            }

            return this;
        }
    }
}
