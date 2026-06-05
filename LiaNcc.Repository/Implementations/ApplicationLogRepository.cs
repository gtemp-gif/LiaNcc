using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Logging;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class ApplicationLogRepository : IApplicationLogRepository
    {
        private readonly LiaNccDbContext _context;

        public ApplicationLogRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ApplicationLog log)
        {
            _context.ApplicationLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ApplicationLog> logs)
        {
            _context.ApplicationLogs.AddRange(logs);
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationLogDto?> GetByIdAsync(long id)
        {
            var log = await _context.ApplicationLogs.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
            return log == null ? null : MapToDto(log);
        }

        public async Task<PaginatedLogsResponse> GetPagedAsync(ApplicationLogFilterRequest filter)
        {
            var query = _context.ApplicationLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(filter.Level)) query = query.Where(l => l.Level == filter.Level);
            if (!string.IsNullOrEmpty(filter.ProjectName)) query = query.Where(l => l.ProjectName == filter.ProjectName);
            if (!string.IsNullOrEmpty(filter.Area)) query = query.Where(l => l.Area == filter.Area);
            if (!string.IsNullOrEmpty(filter.Controller)) query = query.Where(l => l.Controller == filter.Controller);
            if (!string.IsNullOrEmpty(filter.Action)) query = query.Where(l => l.Action == filter.Action);
            if (!string.IsNullOrEmpty(filter.EventType)) query = query.Where(l => l.EventType == filter.EventType);
            if (!string.IsNullOrEmpty(filter.EntityName)) query = query.Where(l => l.EntityName == filter.EntityName);
            if (!string.IsNullOrEmpty(filter.EntityId)) query = query.Where(l => l.EntityId == filter.EntityId);
            if (!string.IsNullOrEmpty(filter.CorrelationId)) query = query.Where(l => l.CorrelationId == filter.CorrelationId);
            if (!string.IsNullOrEmpty(filter.UserId)) query = query.Where(l => l.UserId == filter.UserId);
            if (filter.TenantId.HasValue) query = query.Where(l => l.TenantId == filter.TenantId);
            if (filter.FromDate.HasValue) query = query.Where(l => l.Timestamp >= filter.FromDate.Value);
            if (filter.ToDate.HasValue) query = query.Where(l => l.Timestamp <= filter.ToDate.Value);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(l => l.Message.Contains(filter.SearchTerm)
                                      || (l.Exception != null && l.Exception.Contains(filter.SearchTerm))
                                      || (l.StackTrace != null && l.StackTrace.Contains(filter.SearchTerm))
                                      || (l.AdditionalDataJson != null && l.AdditionalDataJson.Contains(filter.SearchTerm))
                                      || (l.RequestPath != null && l.RequestPath.Contains(filter.SearchTerm))
                                      || (l.Controller != null && l.Controller.Contains(filter.SearchTerm))
                                      || (l.Action != null && l.Action.Contains(filter.SearchTerm)));
            }

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(l => l.Timestamp)
                                   .Skip((filter.Page - 1) * filter.PageSize)
                                   .Take(filter.PageSize)
                                   .ToListAsync();

            return new PaginatedLogsResponse
            {
                Items = items.Select(MapToDto),
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<int> DeleteOlderThanAsync(DateTime olderThan)
        {
            var logsToDelete = await _context.ApplicationLogs.Where(l => l.Timestamp < olderThan).ToListAsync();
            int count = logsToDelete.Count;
            if (count > 0)
            {
                _context.ApplicationLogs.RemoveRange(logsToDelete);
                await _context.SaveChangesAsync();
            }
            return count;
        }

        public async Task<int> CountByLevelAsync(string level, DateTime? fromDate = null)
        {
            var query = _context.ApplicationLogs.Where(l => l.Level == level);
            if (fromDate.HasValue) query = query.Where(l => l.Timestamp >= fromDate.Value);
            return await query.CountAsync();
        }

        public async Task<int> CountErrorsAsync(DateTime fromDate)
        {
            return await _context.ApplicationLogs.CountAsync(l => l.Timestamp >= fromDate && (l.Level == "Error" || l.Level == "Critical" || l.Level == "Fail"));
        }

        private static ApplicationLogDto MapToDto(ApplicationLog log)
        {
            return new ApplicationLogDto
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                Level = log.Level,
                ProjectName = log.ProjectName,
                Area = log.Area,
                Controller = log.Controller,
                Action = log.Action,
                UserId = log.UserId,
                UserName = log.UserName,
                TenantId = log.TenantId,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                EventType = log.EventType,
                Message = log.Message,
                Exception = log.Exception,
                StackTrace = log.StackTrace,
                InnerException = log.InnerException,
                RequestPath = log.RequestPath,
                HttpMethod = log.HttpMethod,
                StatusCode = log.StatusCode,
                IpAddress = log.IpAddress,
                UserAgent = log.UserAgent,
                CorrelationId = log.CorrelationId,
                AdditionalDataJson = log.AdditionalDataJson,
                QueryString = log.QueryString
            };
        }

        // Keep old methods for backward compatibility if interfaces still use them
        public async Task DeleteOldLogsAsync(DateTime olderThan) => await DeleteOlderThanAsync(olderThan);
    }
}
