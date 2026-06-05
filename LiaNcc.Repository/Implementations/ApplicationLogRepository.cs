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

            if (!string.IsNullOrWhiteSpace(filter.Level)) query = query.Where(l => l.Level == filter.Level);
            if (!string.IsNullOrWhiteSpace(filter.ProjectName)) query = query.Where(l => l.ProjectName == filter.ProjectName);
            if (!string.IsNullOrWhiteSpace(filter.Area)) query = query.Where(l => l.Area == filter.Area);
            if (!string.IsNullOrWhiteSpace(filter.Controller)) query = query.Where(l => l.Controller == filter.Controller);
            if (!string.IsNullOrWhiteSpace(filter.Action)) query = query.Where(l => l.Action == filter.Action);
            if (!string.IsNullOrWhiteSpace(filter.EventType)) query = query.Where(l => l.EventType == filter.EventType);
            if (!string.IsNullOrWhiteSpace(filter.EntityName)) query = query.Where(l => l.EntityName == filter.EntityName);
            if (!string.IsNullOrWhiteSpace(filter.EntityId)) query = query.Where(l => l.EntityId == filter.EntityId);
            if (!string.IsNullOrWhiteSpace(filter.CorrelationId)) query = query.Where(l => l.CorrelationId == filter.CorrelationId);
            if (!string.IsNullOrWhiteSpace(filter.UserId)) query = query.Where(l => l.UserId == filter.UserId);
            if (filter.TenantId.HasValue) query = query.Where(l => l.TenantId == filter.TenantId);
            if (filter.FromDate.HasValue) query = query.Where(l => l.Timestamp >= filter.FromDate.Value);
            if (filter.ToDate.HasValue) query = query.Where(l => l.Timestamp <= filter.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                query = query.Where(l => l.Message.Contains(term)
                                      || (l.Exception != null && l.Exception.Contains(term))
                                      || (l.StackTrace != null && l.StackTrace.Contains(term))
                                      || (l.AdditionalDataJson != null && l.AdditionalDataJson.Contains(term))
                                      || (l.RequestPath != null && l.RequestPath.Contains(term))
                                      || (l.Controller != null && l.Controller.Contains(term))
                                      || (l.Action != null && l.Action.Contains(term)));
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
            var count = await _context.ApplicationLogs.Where(l => l.Timestamp < olderThan).ExecuteDeleteAsync();
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

        public async Task DeleteOldLogsAsync(DateTime olderThan) => await DeleteOlderThanAsync(olderThan);
    }
}
