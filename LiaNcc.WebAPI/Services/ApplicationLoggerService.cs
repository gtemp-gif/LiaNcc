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
        private readonly string _source;

        public ApplicationLoggerService(
            IApplicationLogRepository repository,
            ILogger<ApplicationLoggerService> logger,
            IHttpContextAccessor httpContextAccessor,
            string source = "WebAPI")
        {
            _repository = repository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _source = source;
        }

        public async Task LogAsync(CreateApplicationLogRequest request)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var log = new ApplicationLog
                {
                    Source = string.IsNullOrEmpty(request.Source) ? _source : request.Source,
                    Area = request.Area,
                    Action = request.Action,
                    Level = request.Level,
                    Message = request.Message,
                    ExceptionMessage = request.ExceptionMessage,
                    StackTrace = request.StackTrace,
                    InnerException = request.InnerException,
                    UserId = request.UserId ?? httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    UserEmail = request.UserEmail ?? httpContext?.User?.FindFirst(ClaimTypes.Email)?.Value,
                    RequestPath = request.RequestPath ?? httpContext?.Request?.Path,
                    QueryString = request.QueryString ?? httpContext?.Request?.QueryString.ToString(),
                    HttpMethod = request.HttpMethod ?? httpContext?.Request?.Method,
                    StatusCode = request.StatusCode,
                    EntityName = request.EntityName,
                    EntityId = request.EntityId,
                    IpAddress = request.IpAddress ?? httpContext?.Connection?.RemoteIpAddress?.ToString(),
                    UserAgent = request.UserAgent ?? httpContext?.Request?.Headers["User-Agent"].ToString(),
                    CorrelationId = request.CorrelationId ?? httpContext?.Items["CorrelationId"]?.ToString(),
                    AdditionalData = request.AdditionalData,
                    CreatedAt = DateTime.UtcNow
                };

                // Write to database
                await _repository.AddAsync(log);

                // Write to standard logger (file/console via Serilog)
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
                    log.Source, log.Area, log.Action, log.Message, log.ExceptionMessage);
            }
            catch (Exception ex)
            {
                // Fallback to file logger if database fails
                _logger.LogCritical(ex, "FATAL: Database logging failed. Message: {OriginalMessage}", request.Message);
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

        public Task LogCriticalAsync(string area, string action, string message, Exception? exception = null, int? statusCode = null, Guid? entityId = null, string? entityName = null, object? additionalData = null)
        {
            return LogAsync(new CreateApplicationLogRequest
            {
                Level = "Critical",
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
}
