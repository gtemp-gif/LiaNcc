using System;

namespace LiaNcc.Models.Entities
{
    public class ApplicationLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Source { get; set; } = string.Empty;
        public string? Area { get; set; }
        public string? Action { get; set; }
        public string Level { get; set; } = "Info";
        public string Message { get; set; } = string.Empty;
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? RequestPath { get; set; }
        public string? QueryString { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public string? EntityName { get; set; }
        public Guid? EntityId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CorrelationId { get; set; }
        public string? AdditionalData { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
