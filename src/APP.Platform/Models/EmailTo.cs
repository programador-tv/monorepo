using System;
using System.Runtime.ConstrainedExecution;

namespace Models
{
    public class EmailTo
    {
        public string Destination { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public EmailType Type { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}
