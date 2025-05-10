using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine
{
    public partial class OrbitEngine
    {
        #region Methods

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
            if (!string.IsNullOrEmpty(message))
            {
                this._logger.LogWarning("{Message} {Args}", message, args);
            }
        }

        #endregion Methods
    }
}
