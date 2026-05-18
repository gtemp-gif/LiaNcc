using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class SitePageRepository : ISitePageRepository
    {
        private readonly LiaNccDbContext _context;

        public SitePageRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SitePage>> GetAllAsync()
        {
            return await _context.SitePages.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<SitePage>> GetActivePagesAsync()
        {
            return await _context.SitePages.AsNoTracking().Where(p => p.IsActive).ToListAsync();
        }

        public async Task<SitePage?> GetByIdAsync(Guid id)
        {
            return await _context.SitePages.FindAsync(id);
        }

        public async Task<SitePage?> GetBySlugAsync(string slug)
        {
            return await _context.SitePages.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<SitePage?> GetPageWithSectionsAsync(Guid pageId)
        {
            return await _context.SitePages.AsNoTracking()
                .Include(p => p.PageSections.OrderBy(ps => ps.SortOrder))
                .ThenInclude(s => s.CallToActions.OrderBy(c => c.SortOrder))
                .Include(p => p.CallToActions.OrderBy(c => c.SortOrder))
                .FirstOrDefaultAsync(p => p.Id == pageId);
        }

        public async Task<SitePage?> GetPageWithSectionsBySlugAsync(string slug)
        {
            return await _context.SitePages.AsNoTracking()
                .Include(p => p.PageSections.OrderBy(ps => ps.SortOrder))
                .ThenInclude(s => s.CallToActions.OrderBy(c => c.SortOrder))
                .Include(p => p.CallToActions.OrderBy(c => c.SortOrder))
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<SitePage> CreateAsync(SitePage sitePage)
        {
            _context.SitePages.Add(sitePage);
            await _context.SaveChangesAsync();
            return sitePage;
        }

        public async Task UpdateAsync(SitePage sitePage)
        {
            sitePage.UpdatedAt = DateTime.UtcNow;
            _context.SitePages.Update(sitePage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var page = await _context.SitePages.FindAsync(id);
            if (page != null)
            {
                _context.SitePages.Remove(page);
                await _context.SaveChangesAsync();
            }
        }
    }
}
