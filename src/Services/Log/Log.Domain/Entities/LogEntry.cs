

namespace Log.Domain.Entities
{
    public class LogEntry
    {
        public Guid Id { get; private set; }
        public string ServiceName { get; private set; }
        public string Level { get; private set; }
        public string Message { get; private set; }
        public string? Exception { get; private set; }
        public string? StackTrace { get; private set; }
        public Dictionary<string, string> Properties { get; private set; }
        public DateTime Timestamp { get; private set; }

        private LogEntry()
        {
            Properties = new Dictionary<string, string>();
        }

        public LogEntry(
            string serviceName,
            string level,
            string message,
            string? exception = null,
            string? stackTrace = null,
            Dictionary<string, string>? properties = null)
        {
            Id = Guid.NewGuid();
            ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
            Level = level ?? throw new ArgumentNullException(nameof(level));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Exception = exception;
            StackTrace = stackTrace;
            Properties = properties ?? new Dictionary<string, string>();
            Timestamp = DateTime.UtcNow;
        }
    }
}
