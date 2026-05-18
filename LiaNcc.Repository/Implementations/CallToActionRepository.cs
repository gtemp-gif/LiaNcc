using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class CallToActionRepository : ICallToActionRepository
    {
        private readonly LiaNccDbContext _context;

        public CallToActionRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CallToAction>> GetAllAsync()
        {
            return await _context.CallToActions.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<CallToAction>> GetActiveAsync()
        {
            return await _context.CallToActions.AsNoTracking().Where(c => c.IsActive).ToListAsync();
        }

        public async Task<CallToAction?> GetByIdAsync(Guid id)
        {
            return await _context.CallToActions.FindAsync(id);
        }

        public async Task<IEnumerable<CallToAction>> GetByPageAsync(Guid pageId)
        {
            return await _context.CallToActions.AsNoTracking()
                .Where(c => c.PageId == pageId)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<CallToAction>> GetBySectionAsync(Guid sectionId)
        {
            return await _context.CallToActions.AsNoTracking()
                .Where(c => c.SectionId == sectionId)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }

        public async Task<CallToAction> CreateAsync(CallToAction cta)
        {
            _context.CallToActions.Add(cta);
            await _context.SaveChangesAsync();
            return cta;
        }

        public async Task UpdateAsync(CallToAction cta)
        {
            cta.UpdatedAt = DateTime.UtcNow;
            _context.CallToActions.Update(cta);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var cta = await _context.CallToActions.FindAsync(id);
            if (cta != null)
            {
                _context.CallToActions.Remove(cta);
                await _context.SaveChangesAsync();
            }
        }
    }
}
