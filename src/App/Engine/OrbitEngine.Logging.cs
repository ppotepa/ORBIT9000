using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine
{
    public partial class OrbitEngine
    {
<<<<<<< HEAD
        private const string Template = "{Message} {Args}";
        #region Methods

        public void LogCritical(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _logger.LogCritical(Template, message, args);
            }
=======
        public void LogCritical(string message, params object[] args)
        {
            _logger.LogCritical(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }
        public void LogTrace(string message, params object[] args)
        {
            _logger.LogTrace(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
        }

        public void LogDebug(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _logger.LogDebug(Template, message, args);
            }
        }

        public void LogError(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _logger.LogError(Template, message, args);
            }
        }

        public void LogInformation(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _logger.LogInformation(Template, message, args);
            }
        }

        public void LogTrace(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _logger.LogTrace(Template, message, args);
            }
        }

        public void LogWarning(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _logger.LogWarning(Template, message, args);
            }
        }

        #endregion Methods
    }
}
