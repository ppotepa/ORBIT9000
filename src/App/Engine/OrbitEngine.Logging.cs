using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine
{
    public partial class OrbitEngine
    {
<<<<<<< HEAD
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
=======
        #region Methods

>>>>>>> bfa6c2d (Try fix pipeline)
        public void LogCritical(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogCritical("{Message} {Args}", message, args);
            }
        }

        public void LogDebug(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogDebug("{Message} {Args}", message, args);
            }
        }

        public void LogError(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogError("{Message} {Args}", message, args);
            }
        }

        public void LogInformation(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogInformation("{Message} {Args}", message, args);
            }
        }

        public void LogTrace(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogTrace("{Message} {Args}", message, args);
            }
        }

        public void LogWarning(string message, params object[] args)
        {
<<<<<<< HEAD
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

=======
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogWarning("{Message} {Args}", message, args);
            }
        }

>>>>>>> bfa6c2d (Try fix pipeline)
        #endregion Methods
    }
}
