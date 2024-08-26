using Domain.Primitives;

namespace Domain.Entities
{
    public sealed class Log(
        Guid id,
        string message,
        string messageTemplate,
        string level,
        DateTimeOffset timestamp,
        string exception,
        string properties,
        string logEvent,
        DateTime createdAt
    ) : Entity(id, createdAt)
    {
        public string Message { get; private set; } = message;
        public string MessageTemplate { get; private set; } = messageTemplate;
        public string Level { get; private set; } = level;
        public DateTimeOffset Timestamp { get; private set; } = timestamp;
        public string Exception { get; private set; } = exception;
        public string Properties { get; private set; } = properties;
        public string LogEvent { get; private set; } = logEvent;
    }
}
