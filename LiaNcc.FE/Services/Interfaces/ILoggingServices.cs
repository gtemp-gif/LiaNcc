using System;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Logging;

namespace LiaNcc.FE.Services.Interfaces
{
    public interface IApplicationLoggerService
    {
        Task LogAsync(CreateApplicationLogRequest request);
        Task LogInfoAsync(string area, string action, string message, object? additionalData = null);
        Task LogErrorAsync(string area, string action, string message, Exception? exception = null, object? additionalData = null);
    }

    public interface ILogsApiClient
    {
        Task CreateLogAsync(CreateApplicationLogRequest request);
    }
}
