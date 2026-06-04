using System;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Logging;

namespace LiaNcc.WebAPI.Services
{
    public interface IApplicationLoggerService
    {
        Task LogAsync(CreateApplicationLogRequest request);
        Task LogInfoAsync(string area, string action, string message, Guid? entityId = null, string? entityName = null, object? additionalData = null);
        Task LogWarningAsync(string area, string action, string message, Guid? entityId = null, string? entityName = null, object? additionalData = null);
        Task LogErrorAsync(string area, string action, string message, Exception? exception = null, int? statusCode = null, Guid? entityId = null, string? entityName = null, object? additionalData = null);
        Task LogCriticalAsync(string area, string action, string message, Exception? exception = null, int? statusCode = null, Guid? entityId = null, string? entityName = null, object? additionalData = null);
    }
}
