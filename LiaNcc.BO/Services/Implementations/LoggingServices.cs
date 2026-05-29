using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.DTOs.Logging;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LiaNcc.BO.Services.Implementations
{
    public class ApplicationLoggerService : IApplicationLoggerService
    {
        private readonly ILogsApiClient _logsApiClient;
        private readonly ILogger<ApplicationLoggerService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _projectName;

        public ApplicationLoggerService(
            ILogsApiClient logsApiClient,
            ILogger<ApplicationLoggerService> logger,
            IHttpContextAccessor httpContextAccessor,
            string projectName = "LiaNcc.BO")
        {
            _logsApiClient = logsApiClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _projectName = projectName;
        }

        public async Task LogAsync(CreateApplicationLogRequest request)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                request.ProjectName = string.IsNullOrEmpty(request.ProjectName) ? _projectName : request.ProjectName;
                request.UserId ??= httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                request.UserName ??= httpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
                request.CorrelationId ??= httpContext?.Items["CorrelationId"]?.ToString();
                request.RequestPath ??= httpContext?.Request?.Path;
                request.HttpMethod ??= httpContext?.Request?.Method;
                request.IpAddress ??= httpContext?.Connection?.RemoteIpAddress?.ToString();
                request.UserAgent ??= httpContext?.Request?.Headers["User-Agent"].ToString();

                // 1. Write to standard logger (file/console via Serilog)
                var logLevel = request.Level switch
                {
                    "Trace" => LogLevel.Trace,
                    "Debug" => LogLevel.Debug,
                    "Information" => LogLevel.Information,
                    "Warning" => LogLevel.Warning,
                    "Error" => LogLevel.Error,
                    "Critical" => LogLevel.Critical,
                    _ => LogLevel.Information
                };

                _logger.Log(logLevel, "[{ProjectName}] {Area}.{Controller}.{Action} - {EventType}: {Message} {Exception}",
                    request.ProjectName, request.Area, request.Controller, request.Action, request.EventType, request.Message, request.Exception);

                // 2. Send to WebAPI
                await _logsApiClient.CreateLogAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "FATAL: API logging failed from BO. Message: {OriginalMessage}", request.Message);
            }
        }

        public async Task LogInformationAsync(string area, string action, string message, string? controller = null, string? entityName = null, object? entityId = null, string? eventType = null, object? additionalData = null)
        {
            await LogAsync(new CreateApplicationLogRequest
            {
                Level = "Information",
                Area = area,
                Controller = controller,
                Action = action,
                Message = message,
                EntityName = entityName,
                EntityId = entityId?.ToString(),
                EventType = eventType,
                AdditionalDataJson = additionalData != null ? JsonSerializer.Serialize(additionalData) : null
            });
        }

        public async Task LogWarningAsync(string area, string action, string message, string? controller = null, string? entityName = null, object? entityId = null, string? eventType = null, object? additionalData = null)
        {
            await LogAsync(new CreateApplicationLogRequest
            {
                Level = "Warning",
                Area = area,
                Controller = controller,
                Action = action,
                Message = message,
                EntityName = entityName,
                EntityId = entityId?.ToString(),
                EventType = eventType,
                AdditionalDataJson = additionalData != null ? JsonSerializer.Serialize(additionalData) : null
            });
        }

        public async Task LogErrorAsync(string area, string action, string message, Exception? exception = null, int? statusCode = null, string? controller = null, string? entityName = null, object? entityId = null, string? eventType = "Exception", object? additionalData = null)
        {
            await LogAsync(new CreateApplicationLogRequest
            {
                Level = "Error",
                Area = area,
                Controller = controller,
                Action = action,
                Message = message,
                Exception = exception?.Message,
                StackTrace = exception?.StackTrace,
                StatusCode = statusCode,
                EntityName = entityName,
                EntityId = entityId?.ToString(),
                EventType = eventType,
                AdditionalDataJson = additionalData != null ? JsonSerializer.Serialize(additionalData) : null
            });
        }

        // Backward compatibility
        public Task LogInfoAsync(string area, string action, string message, Guid? entityId = null, string? entityName = null, object? additionalData = null)
        {
            return LogInformationAsync(area, action, message, null, entityName, entityId, null, additionalData);
        }
    }

    public class LogsApiClient : BaseApiClient<ApplicationLog, long>, ILogsApiClient
    {
        public LogsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "logs") { }

        public async Task<PaginatedLogsResponse> GetLogsAsync(ApplicationLogFilterRequest filter)
        {
            SetBearerToken();
            var queryParams = new List<string>
            {
                $"projectName={Uri.EscapeDataString(filter.ProjectName ?? "")}",
                $"area={Uri.EscapeDataString(filter.Area ?? "")}",
                $"level={Uri.EscapeDataString(filter.Level ?? "")}",
                $"controller={Uri.EscapeDataString(filter.Controller ?? "")}",
                $"action={Uri.EscapeDataString(filter.Action ?? "")}",
                $"eventType={Uri.EscapeDataString(filter.EventType ?? "")}",
                $"entityName={Uri.EscapeDataString(filter.EntityName ?? "")}",
                $"entityId={Uri.EscapeDataString(filter.EntityId ?? "")}",
                $"correlationId={Uri.EscapeDataString(filter.CorrelationId ?? "")}",
                $"userId={Uri.EscapeDataString(filter.UserId ?? "")}",
                $"searchTerm={Uri.EscapeDataString(filter.SearchTerm ?? "")}",
                $"page={filter.Page}",
                $"pageSize={filter.PageSize}"
            };

            if (filter.TenantId.HasValue) queryParams.Add($"tenantId={filter.TenantId}");
            if (filter.FromDate.HasValue) queryParams.Add($"fromDate={filter.FromDate.Value:yyyy-MM-dd}");
            if (filter.ToDate.HasValue) queryParams.Add($"toDate={filter.ToDate.Value:yyyy-MM-dd}");

            var query = "?" + string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{_endpointUrl}{query}");
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<PaginatedLogsResponse>(_jsonSerializerOptions) ?? new PaginatedLogsResponse();
        }

        public async Task<ApplicationLogDto?> GetLogByIdAsync(long id)
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"{_endpointUrl}/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<ApplicationLogDto>(_jsonSerializerOptions);
        }

        public async Task CreateLogAsync(CreateApplicationLogRequest request)
        {
            SetBearerToken();
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_endpointUrl, request, _jsonSerializerOptions);
                // Don't throw for logging failures to avoid app crash
            }
            catch
            {
                // Silently ignore
            }
        }

        public async Task CleanupAsync(int olderThanDays)
        {
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"{_endpointUrl}/cleanup?olderThanDays={olderThanDays}");
            EnsureValidResponse(response);
        }

        public async Task<object?> GetStatsAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"{_endpointUrl}/stats");
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<object>(_jsonSerializerOptions);
        }
    }
}
