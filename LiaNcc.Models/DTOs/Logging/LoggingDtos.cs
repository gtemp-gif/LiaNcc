using System;
using System.Collections.Generic;

namespace LiaNcc.Models.DTOs.Logging
{
    public class ApplicationLogDto
    {
        public Guid Id { get; set; }
        public string Source { get; set; } = string.Empty;
        public string? Area { get; set; }
        public string? Action { get; set; }
        public string Level { get; set; } = string.Empty;
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
        public DateTime CreatedAt { get; set; }
    }

    public class CreateApplicationLogRequest
    {
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
    }

    public class ApplicationLogFilterRequest
    {
        public string? Source { get; set; }
        public string? Area { get; set; }
        public string? Level { get; set; }
        public string? Action { get; set; }
        public string? EntityName { get; set; }
        public Guid? EntityId { get; set; }
        public string? CorrelationId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
