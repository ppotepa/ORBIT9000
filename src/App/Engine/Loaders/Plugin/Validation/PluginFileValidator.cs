using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine.Loaders.Plugin.Validation
{
    /// <summary>
    /// Helper class to validate plugin files.
    /// </summary>
    internal class PluginFileValidator
    {
        private readonly ILogger? _logger;
        private readonly FileInfo _info;

        public PluginFileValidator(FileInfo info, ILogger? logger)
        {
            _info = info ?? throw new ArgumentNullException(nameof(info));
            _logger = logger;
        }

        public bool FileExists => _info.Exists;
        public bool IsDll => string.Equals(_info.Extension, ".dll", StringComparison.OrdinalIgnoreCase);
        public bool IsValid => FileExists && IsDll;
        public List<Exception> Exceptions = new List<Exception>();  

        /// <summary>
        /// Validates the plugin file Path and type.
        /// </summary>
        /// <returns>A list of exceptions encountered during validation.</returns>
        public PluginFileValidator Validate()
        {
            if (_info == null)
            {
                _logger?.LogWarning("FileInfo object is null.");
                Exceptions.Add(new ArgumentNullException(nameof(_info), "FileInfo object is null."));                
            }

            if (!FileExists)
            {
                _logger?.LogWarning("File does not exist: {FilePath}", _info.FullName);
                Exceptions.Add(new FileNotFoundException($"File does not exist: {_info.FullName}"));
            }

            if (!IsDll)
            {
                _logger?.LogWarning("File is not a DLL: {FilePath}", _info.FullName);
                Exceptions.Add(new ArgumentException($"File is not a DLL: {_info.FullName}"));
            }

            return this;
        }
    }
}
