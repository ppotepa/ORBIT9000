using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine.Tests.TestHelpers.Logging
{
    public class LogEntry
    {
        public string? Category { get; set; }
        public LogLevel? Level { get; set; }
        public string? Message { get; set; }
    }
}