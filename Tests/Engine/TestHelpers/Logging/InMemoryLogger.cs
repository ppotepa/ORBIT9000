using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Environment;
namespace ORBIT9000.Engine.Tests.TestHelpers.Logging
{
    public class InMemoryLogger(string category, List<LogEntry> entries) : ILogger
    {
        private readonly string _category = category;
        private readonly List<LogEntry> _entries = entries;

        public IDisposable? BeginScope<TState>(TState state)
             where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                                Exception? exception, Func<TState, Exception?, string> formatter)
        {
<<<<<<< HEAD
            _entries.Add(new LogEntry
            {
                Category = _category,
=======
            this._entries.Add(new LogEntry
            {
                Category = this._category,
>>>>>>> bfa6c2d (Try fix pipeline)
                Level = logLevel!,
                Message = formatter(state, exception)
            });
        }

        private class NullScope : Disposable
        {
            public static NullScope Instance { get; } = new NullScope();

            protected override void Dispose(bool disposing)
            {
<<<<<<< HEAD
                if (!disposed)
=======
                if (!this.disposed)
>>>>>>> bfa6c2d (Try fix pipeline)
                {
                    if (disposing)
                    {
                        Instance.Dispose();
                    }

<<<<<<< HEAD
                    disposed = true;
=======
                    this.disposed = true;
>>>>>>> bfa6c2d (Try fix pipeline)
                }

                base.Dispose(disposing);
            }
        }
    }
}
