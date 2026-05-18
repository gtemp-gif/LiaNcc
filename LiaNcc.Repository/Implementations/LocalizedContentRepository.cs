using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class LocalizedContentRepository : ILocalizedContentRepository
    {
        private readonly LiaNccDbContext _context;

        public LocalizedContentRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LocalizedContent>> GetAllAsync()
        {
            return await _context.LocalizedContents.AsNoTracking().ToListAsync();
        }

        public async Task<LocalizedContent?> GetByIdAsync(Guid id)
        {
            return await _context.LocalizedContents.FindAsync(id);
        }

        public async Task<IEnumerable<LocalizedContent>> GetByEntityAsync(string entityName, Guid entityId, string languageCode)
        {
            return await _context.LocalizedContents.AsNoTracking()
                .Where(lc => lc.EntityName == entityName && lc.EntityId == entityId && lc.LanguageCode == languageCode)
                .ToListAsync();
        }

        public async Task<LocalizedContent?> GetStaticContentAsync(string contentKey, string languageCode)
        {
            return await _context.LocalizedContents.AsNoTracking()
                .FirstOrDefaultAsync(lc => lc.EntityName == "Static" && lc.ContentKey == contentKey && lc.LanguageCode == languageCode);
        }

        public async Task<IEnumerable<LocalizedContent>> GetStaticContentsAsync(string languageCode)
        {
            return await _context.LocalizedContents.AsNoTracking()
                .Where(lc => lc.EntityName == "Static" && lc.LanguageCode == languageCode)
                .ToListAsync();
        }

        public async Task<LocalizedContent> CreateAsync(LocalizedContent localizedContent)
        {
            _context.LocalizedContents.Add(localizedContent);
            await _context.SaveChangesAsync();
            return localizedContent;
        }

        public async Task UpdateAsync(LocalizedContent localizedContent)
        {
            localizedContent.UpdatedAt = DateTime.UtcNow;
            _context.LocalizedContents.Update(localizedContent);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var content = await _context.LocalizedContents.FindAsync(id);
            if (content != null)
            {
                _context.LocalizedContents.Remove(content);
                await _context.SaveChangesAsync();
            }
        }
    }
}
