using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiaNcc.Models
{
    public enum ApplicationLogLevel
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Critical
    }

    public enum ApplicationEventType
    {
        Create,
        Update,
        Delete,
        Login,
        Logout,
        Import,
        EnrichAI,
        PairingCalculation,
        PairingDiscovery,
        MediaUpload,
        MediaDelete,
        SettingsChange,
        SubscriptionChange,
        TenantChange,
        SystemAction,
        Exception
    }

    public class ApplicationLog
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Level { get; set; } = string.Empty;
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
        public string? RequestPath { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CorrelationId { get; set; }
        public string? AdditionalDataJson { get; set; }
    }

    public class ApplicationLogDto
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
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
        public string? RequestPath { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CorrelationId { get; set; }
        public string? AdditionalDataJson { get; set; }
    }

    public class CreateApplicationLogRequest
    {
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
        public string? RequestPath { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CorrelationId { get; set; }
        public string? AdditionalDataJson { get; set; }
    }

    public class ApplicationLogFilterRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Level { get; set; }
        public string? ProjectName { get; set; }
        public Guid? TenantId { get; set; }
        public string? UserId { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? EventType { get; set; }
        public string? EntityName { get; set; }
        public string? CorrelationId { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class PaginatedLogsResponse
    {
        public IEnumerable<ApplicationLogDto> Items { get; set; } = Enumerable.Empty<ApplicationLogDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

}
