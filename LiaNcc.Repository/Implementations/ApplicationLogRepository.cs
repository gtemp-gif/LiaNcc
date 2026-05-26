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

        public async Task<ApplicationLog?> GetByIdAsync(Guid id)
        {
            return await _context.ApplicationLogs.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<PagedResult<ApplicationLog>> GetPagedAsync(ApplicationLogFilterRequest filter)
        {
            var query = _context.ApplicationLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(filter.Source)) query = query.Where(l => l.Source == filter.Source);
            if (!string.IsNullOrEmpty(filter.Area)) query = query.Where(l => l.Area == filter.Area);
            if (!string.IsNullOrEmpty(filter.Level)) query = query.Where(l => l.Level == filter.Level);
            if (!string.IsNullOrEmpty(filter.Action)) query = query.Where(l => l.Action == filter.Action);
            if (!string.IsNullOrEmpty(filter.EntityName)) query = query.Where(l => l.EntityName == filter.EntityName);
            if (filter.EntityId.HasValue) query = query.Where(l => l.EntityId == filter.EntityId);
            if (!string.IsNullOrEmpty(filter.CorrelationId)) query = query.Where(l => l.CorrelationId == filter.CorrelationId);
            if (filter.FromDate.HasValue) query = query.Where(l => l.CreatedAt >= filter.FromDate.Value);
            if (filter.ToDate.HasValue) query = query.Where(l => l.CreatedAt <= filter.ToDate.Value);

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(l => (l.Message != null && l.Message.Contains(filter.Search))
                                      || (l.ExceptionMessage != null && l.ExceptionMessage.Contains(filter.Search))
                                      || (l.UserEmail != null && l.UserEmail.Contains(filter.Search)));
            }

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(l => l.CreatedAt)
                                   .Skip((filter.Page - 1) * filter.PageSize)
                                   .Take(filter.PageSize)
                                   .ToListAsync();

            return new PagedResult<ApplicationLog>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task DeleteOldLogsAsync(DateTime olderThan)
        {
            var oldLogs = await _context.ApplicationLogs.Where(l => l.CreatedAt < olderThan).ToListAsync();
            _context.ApplicationLogs.RemoveRange(oldLogs);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountErrorsAsync(DateTime fromDate)
        {
            return await _context.ApplicationLogs.CountAsync(l => l.CreatedAt >= fromDate && (l.Level == "Error" || l.Level == "Critical"));
        }

        public async Task<int> CountByLevelAsync(string level, DateTime? fromDate = null)
        {
            var query = _context.ApplicationLogs.Where(l => l.Level == level);
            if (fromDate.HasValue) query = query.Where(l => l.CreatedAt >= fromDate.Value);
            return await query.CountAsync();
        }
    }
}
