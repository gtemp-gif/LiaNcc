using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using LiaNcc.FE.Services.Interfaces;
using LiaNcc.Models.DTOs.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LiaNcc.FE.Services.Implementations
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
            string projectName = "LiaNcc.FE")
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
                request.CorrelationId ??= httpContext?.Items["CorrelationId"]?.ToString();
                request.UserId ??= httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                request.UserName ??= httpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
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
                _logger.LogCritical(ex, "FATAL: API logging failed from FE. Message: {OriginalMessage}", request.Message);
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
        public Task LogInfoAsync(string area, string action, string message, object? additionalData = null)
        {
            return LogInformationAsync(area, action, message, null, null, null, null, additionalData);
        }
    }

    public class LogsApiClient : ILogsApiClient
    {
        private readonly HttpClient _httpClient;

        public LogsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateLogAsync(CreateApplicationLogRequest request)
        {
            try
            {
                await _httpClient.PostAsJsonAsync("logs", request);
            }
            catch
            {
                // Ignore errors to avoid loop or breaking the app
            }
        }
    }
}
