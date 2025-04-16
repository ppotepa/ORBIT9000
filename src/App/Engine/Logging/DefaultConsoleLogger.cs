using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine.Logging
{
    internal class DefaultConsoleLogger : ILogger<OrbitEngine>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;


        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Console.WriteLine
            ( 
                $"[{DateTime.Now.Date.ToShortDateString()}][{DateTime.UtcNow.TimeOfDay}]ID:{eventId}, {logLevel}, {(dynamic)state!}"
            );
        }
    }
}
