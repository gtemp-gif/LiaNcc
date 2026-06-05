using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Logging;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface IApplicationLogRepository
    {
        Task AddAsync(ApplicationLog log);
        Task AddRangeAsync(IEnumerable<ApplicationLog> logs);
        Task<ApplicationLogDto?> GetByIdAsync(long id);
        Task<PaginatedLogsResponse> GetPagedAsync(ApplicationLogFilterRequest filter);
        Task<int> DeleteOlderThanAsync(DateTime olderThan);
        Task<int> CountErrorsAsync(DateTime fromDate);
        Task<int> CountByLevelAsync(string level, DateTime? fromDate = null);

        // Backward compatibility
        Task DeleteOldLogsAsync(DateTime olderThan);
    }
}
