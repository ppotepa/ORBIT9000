using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine.Exceptions
{
    public class ExceptionFactory
    {
        private readonly ILogger _logger;
        private readonly bool _abortOnError;

        public ExceptionFactory(ILogger logger, bool abortOnError)
        {
            _logger = logger;
            _abortOnError = abortOnError;
        }

        public void ThrowIfNecessary(Exception exception, string message = null)
        {
            if (message != null)
            {
                _logger.LogError(exception, message);
            }
            else
            {
                _logger.LogError(exception, exception.Message);
            }

            if (_abortOnError)
            {
                throw exception;
            }
        }

        public void ThrowIfNecessary(string message)
        {
            _logger.LogError(message);

            if (_abortOnError)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
