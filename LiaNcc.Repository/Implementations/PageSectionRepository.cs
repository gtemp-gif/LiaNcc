using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class PageSectionRepository : IPageSectionRepository
    {
        private readonly LiaNccDbContext _context;

        public PageSectionRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PageSection>> GetAllAsync()
        {
            return await _context.PageSections.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<PageSection>> GetActiveSectionsAsync()
        {
            return await _context.PageSections.AsNoTracking().Where(s => s.IsActive).ToListAsync();
        }

        public async Task<PageSection?> GetByIdAsync(Guid id)
        {
            return await _context.PageSections.FindAsync(id);
        }

        public async Task<IEnumerable<PageSection>> GetSectionsByPageAsync(Guid pageId)
        {
            return await _context.PageSections.AsNoTracking()
                .Where(s => s.PageId == pageId)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();
        }

        public async Task<PageSection> CreateAsync(PageSection pageSection)
        {
            _context.PageSections.Add(pageSection);
            await _context.SaveChangesAsync();
            return pageSection;
        }

        public async Task UpdateAsync(PageSection pageSection)
        {
            pageSection.UpdatedAt = DateTime.UtcNow;
            _context.PageSections.Update(pageSection);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var section = await _context.PageSections.FindAsync(id);
            if (section != null)
            {
                _context.PageSections.Remove(section);
                await _context.SaveChangesAsync();
            }
        }
    }
}
