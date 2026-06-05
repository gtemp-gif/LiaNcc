using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Logging;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LiaNcc.WebAPI.Services
{
    public class ApplicationLoggerService : IApplicationLoggerService
    {
        private readonly IApplicationLogRepository _repository;
        private readonly ILogger<ApplicationLoggerService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _projectName;

        public ApplicationLoggerService(
            IApplicationLogRepository repository,
            ILogger<ApplicationLoggerService> logger,
            IHttpContextAccessor httpContextAccessor,
            string projectName = "LiaNcc.WebAPI")
        {
            _repository = repository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _projectName = projectName;
        }

        public async Task LogAsync(CreateApplicationLogRequest request)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var log = new ApplicationLog
                {
                    ProjectName = string.IsNullOrEmpty(request.ProjectName) ? _projectName : request.ProjectName,
                    Area = request.Area,
                    Controller = request.Controller,
                    Action = request.Action,
                    Level = request.Level,
                    Message = request.Message,
                    Exception = request.Exception,
                    StackTrace = request.StackTrace,
                    UserId = request.UserId ?? httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    UserName = request.UserName ?? httpContext?.User?.FindFirst(ClaimTypes.Email)?.Value,
                    TenantId = request.TenantId,
                    RequestPath = request.RequestPath ?? httpContext?.Request?.Path,
                    HttpMethod = request.HttpMethod ?? httpContext?.Request?.Method,
                    StatusCode = request.StatusCode,
                    EntityName = request.EntityName,
                    EntityId = request.EntityId,
                    EventType = request.EventType,
                    IpAddress = request.IpAddress ?? httpContext?.Connection?.RemoteIpAddress?.ToString(),
                    UserAgent = request.UserAgent ?? httpContext?.Request?.Headers["User-Agent"].ToString(),
                    CorrelationId = request.CorrelationId ?? httpContext?.Items["CorrelationId"]?.ToString(),
                    AdditionalDataJson = request.AdditionalDataJson,
                    Timestamp = DateTime.UtcNow
                };

                // Write to database
                await _repository.AddAsync(log);

                // Write to standard logger (file/console via Serilog)
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
                    log.ProjectName, log.Area, log.Controller, log.Action, log.EventType, log.Message, log.Exception);
            }
            catch (Exception ex)
            {
                // Fallback to file logger if database fails
                _logger.LogCritical(ex, "FATAL: Database logging failed. Message: {OriginalMessage}", request.Message);
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

        public async Task LogCriticalAsync(string area, string action, string message, Exception? exception = null, int? statusCode = null, string? controller = null, string? entityName = null, object? entityId = null, string? eventType = "CriticalException", object? additionalData = null)
        {
            await LogAsync(new CreateApplicationLogRequest
            {
                Level = "Critical",
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
}
