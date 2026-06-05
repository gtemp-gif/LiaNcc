using System;

namespace LiaNcc.Models.Entities
{
    public class ApplicationLog
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Level { get; set; } = "Information";
        public string ProjectName { get; set; } = string.Empty;
        public string? Area { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public Guid? TenantId { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? EventType { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
        public string? RequestPath { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CorrelationId { get; set; }
        public string? AdditionalDataJson { get; set; }
        public string? QueryString { get; set; }

        // Compatibility properties for old code
        public string Source { get => ProjectName; set => ProjectName = value; }
        public DateTime CreatedAt { get => Timestamp; set => Timestamp = value; }
        public string? ExceptionMessage { get => Exception; set => Exception = value; }
        public string? AdditionalData { get => AdditionalDataJson; set => AdditionalDataJson = value; }
        public string? UserEmail { get => UserName; set => UserName = value; }
    }
}
