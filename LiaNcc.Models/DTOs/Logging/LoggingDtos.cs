using System;
using System.Collections.Generic;

namespace LiaNcc.Models.DTOs.Logging
{
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
        public string? InnerException { get; set; }
        public string? RequestPath { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CorrelationId { get; set; }
        public string? AdditionalDataJson { get; set; }
        public string? QueryString { get; set; }

        // Compatibility
        public string Source { get => ProjectName; set => ProjectName = value; }
        public DateTime CreatedAt { get => Timestamp; set => Timestamp = value; }
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
        public string? InnerException { get; set; }
        public string? RequestPath { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CorrelationId { get; set; }
        public string? AdditionalDataJson { get; set; }
        public string? QueryString { get; set; }

        // Compatibility
        public string Source { get => ProjectName; set => ProjectName = value; }
        public string? ExceptionMessage { get => Exception; set => Exception = value; }
        public string? AdditionalData { get => AdditionalDataJson; set => AdditionalDataJson = value; }
    }

    public class ApplicationLogFilterRequest
    {
        public string? Level { get; set; }
        public string? ProjectName { get; set; }
        public string? Area { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? EventType { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? CorrelationId { get; set; }
        public string? UserId { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        // Compatibility
        public string? Source { get => ProjectName; set => ProjectName = value; }
        public string? Search { get => SearchTerm; set => SearchTerm = value; }
    }

    public class PaginatedLogsResponse
    {
        public IEnumerable<ApplicationLogDto> Items { get; set; } = new List<ApplicationLogDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    // For backward compatibility where PagedResult<ApplicationLog> might be used
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
