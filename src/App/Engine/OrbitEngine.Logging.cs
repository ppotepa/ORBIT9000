using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine
{
    public partial class OrbitEngine
    {
        private const string Template = "{Message} {Args}";
        #region Methods

        public void LogCritical(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogCritical(Template, message, args);
            }
        }

        public void LogDebug(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogDebug(Template, message, args);
            }
        }

        public void LogError(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogError(Template, message, args);
            }
        }

        public void LogInformation(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogInformation(Template, message, args);
            }
        }

        public void LogTrace(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogTrace(Template, message, args);
            }
        }

        public void LogWarning(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogWarning(Template, message, args);
            }
        }

        #endregion Methods
    }
}
