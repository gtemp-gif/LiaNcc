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
        private readonly string _source;

        public ApplicationLoggerService(
            ILogsApiClient logsApiClient,
            ILogger<ApplicationLoggerService> logger,
            IHttpContextAccessor httpContextAccessor,
            string source = "BO")
        {
            _logsApiClient = logsApiClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _source = source;
        }

        public async Task LogAsync(CreateApplicationLogRequest request)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                request.Source = string.IsNullOrEmpty(request.Source) ? _source : request.Source;
                request.UserId ??= httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                request.UserEmail ??= httpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
                request.CorrelationId ??= httpContext?.Items["CorrelationId"]?.ToString();

                // 1. Write to standard logger (file/console via Serilog)
                var logLevel = request.Level switch
                {
                    "Trace" => LogLevel.Trace,
                    "Debug" => LogLevel.Debug,
                    "Info" => LogLevel.Information,
                    "Warning" => LogLevel.Warning,
                    "Error" => LogLevel.Error,
                    "Critical" => LogLevel.Critical,
                    _ => LogLevel.Information
                };

                _logger.Log(logLevel, "[{Source}] {Area} - {Action}: {Message} {Exception}",
                    request.Source, request.Area, request.Action, request.Message, request.ExceptionMessage);

                // 2. Send to WebAPI
                await _logsApiClient.CreateLogAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "FATAL: API logging failed from BO. Message: {OriginalMessage}", request.Message);
            }
        }

        public Task LogInfoAsync(string area, string action, string message, Guid? entityId = null, string? entityName = null, object? additionalData = null)
        {
            return LogAsync(new CreateApplicationLogRequest
            {
                Level = "Info",
                Area = area,
                Action = action,
                Message = message,
                EntityId = entityId,
                EntityName = entityName,
                AdditionalData = additionalData != null ? JsonSerializer.Serialize(additionalData) : null
            });
        }

        public Task LogWarningAsync(string area, string action, string message, Guid? entityId = null, string? entityName = null, object? additionalData = null)
        {
            return LogAsync(new CreateApplicationLogRequest
            {
                Level = "Warning",
                Area = area,
                Action = action,
                Message = message,
                EntityId = entityId,
                EntityName = entityName,
                AdditionalData = additionalData != null ? JsonSerializer.Serialize(additionalData) : null
            });
        }

        public Task LogErrorAsync(string area, string action, string message, Exception? exception = null, int? statusCode = null, Guid? entityId = null, string? entityName = null, object? additionalData = null)
        {
            return LogAsync(new CreateApplicationLogRequest
            {
                Level = "Error",
                Area = area,
                Action = action,
                Message = message,
                ExceptionMessage = exception?.Message,
                StackTrace = exception?.StackTrace,
                InnerException = exception?.InnerException?.Message,
                StatusCode = statusCode,
                EntityId = entityId,
                EntityName = entityName,
                AdditionalData = additionalData != null ? JsonSerializer.Serialize(additionalData) : null
            });
        }
    }

    public class LogsApiClient : BaseApiClient<ApplicationLog, Guid>, ILogsApiClient
    {
        public LogsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "logs") { }

        public async Task<PagedResult<ApplicationLog>> GetLogsAsync(ApplicationLogFilterRequest filter)
        {
            SetBearerToken();
            var queryParams = new List<string>
            {
                $"source={Uri.EscapeDataString(filter.Source ?? "")}",
                $"area={Uri.EscapeDataString(filter.Area ?? "")}",
                $"level={Uri.EscapeDataString(filter.Level ?? "")}",
                $"action={Uri.EscapeDataString(filter.Action ?? "")}",
                $"entityName={Uri.EscapeDataString(filter.EntityName ?? "")}",
                $"correlationId={Uri.EscapeDataString(filter.CorrelationId ?? "")}",
                $"search={Uri.EscapeDataString(filter.Search ?? "")}",
                $"page={filter.Page}",
                $"pageSize={filter.PageSize}"
            };

            if (filter.EntityId.HasValue) queryParams.Add($"entityId={filter.EntityId}");
            if (filter.FromDate.HasValue) queryParams.Add($"fromDate={filter.FromDate.Value:yyyy-MM-dd}");
            if (filter.ToDate.HasValue) queryParams.Add($"toDate={filter.ToDate.Value:yyyy-MM-dd}");

            var query = "?" + string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{_endpointUrl}{query}");
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<PagedResult<ApplicationLog>>(_jsonSerializerOptions) ?? new PagedResult<ApplicationLog>();
        }

        public async Task<ApplicationLog?> GetLogByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task CreateLogAsync(CreateApplicationLogRequest request)
        {
            SetBearerToken();
            var response = await _httpClient.PostAsJsonAsync(_endpointUrl, request, _jsonSerializerOptions);
            // Don't use EnsureValidResponse here to avoid infinite loops or crashes during logging
            response.EnsureSuccessStatusCode();
        }

        public async Task CleanupAsync(int olderThanDays)
        {
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"{_endpointUrl}/cleanup?olderThanDays={olderThanDays}");
            EnsureValidResponse(response);
        }

        public async Task<object> GetStatsAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"{_endpointUrl}/stats");
            EnsureValidResponse(response);
            return (await response.Content.ReadFromJsonAsync<object>(_jsonSerializerOptions))!;
        }
    }
}
