using System;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Logging;

namespace LiaNcc.FE.Services.Interfaces
{
    public interface IApplicationLoggerService
    {
        Task LogAsync(CreateApplicationLogRequest request);

        Task LogInformationAsync(
            string area,
            string action,
            string message,
            string? controller = null,
            string? entityName = null,
            object? entityId = null,
            string? eventType = null,
            object? additionalData = null);

        Task LogWarningAsync(
            string area,
            string action,
            string message,
            string? controller = null,
            string? entityName = null,
            object? entityId = null,
            string? eventType = null,
            object? additionalData = null);

        Task LogErrorAsync(
            string area,
            string action,
            string message,
            Exception? exception = null,
            int? statusCode = null,
            string? controller = null,
            string? entityName = null,
            object? entityId = null,
            string? eventType = "Exception",
            object? additionalData = null);

        // Backward compatibility
        Task LogInfoAsync(string area, string action, string message, object? additionalData = null);
    }

    public interface ILogsApiClient
    {
        Task CreateLogAsync(CreateApplicationLogRequest request);
    }
}
