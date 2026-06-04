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
        private readonly string _source;

        public ApplicationLoggerService(
            ILogsApiClient logsApiClient,
            ILogger<ApplicationLoggerService> logger,
            IHttpContextAccessor httpContextAccessor,
            string source = "FE")
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
                _logger.LogCritical(ex, "FATAL: API logging failed from FE. Message: {OriginalMessage}", request.Message);
            }
        }

        public Task LogInfoAsync(string area, string action, string message, object? additionalData = null)
        {
            return LogAsync(new CreateApplicationLogRequest
            {
                Level = "Info",
                Area = area,
                Action = action,
                Message = message,
                AdditionalData = additionalData != null ? JsonSerializer.Serialize(additionalData) : null
            });
        }

        public Task LogErrorAsync(string area, string action, string message, Exception? exception = null, object? additionalData = null)
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
                AdditionalData = additionalData != null ? JsonSerializer.Serialize(additionalData) : null
            });
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
            await _httpClient.PostAsJsonAsync("logs", request);
        }
    }
}
