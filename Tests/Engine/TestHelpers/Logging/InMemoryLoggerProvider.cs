using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Environment;
using System.Collections.Concurrent;

namespace ORBIT9000.Engine.Tests.TestHelpers.Logging
{
    public class InMemoryLoggerProvider : Disposable, ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, InMemoryLogger> _loggers
            = new();

        public List<LogEntry> Entries { get; } = [];

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new InMemoryLogger(name, Entries));

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _loggers.Clear();
                    Entries.Clear();
                }

                base.Dispose(disposing);
            }
        }

        internal ILoggerFactory CreateLoggerFactory()
        {
            LoggerFactory loggerFactory = new();
            loggerFactory.AddProvider(this);
            return loggerFactory;
        }
    }
}
