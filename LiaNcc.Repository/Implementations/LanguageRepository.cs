using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly LiaNccDbContext _context;

        public LanguageRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Language>> GetAllAsync()
        {
            return await _context.Languages.AsNoTracking().ToListAsync();
        }

        public async Task<Language?> GetByIdAsync(Guid id)
        {
            return await _context.Languages.FindAsync(id);
        }

        public async Task<Language?> GetDefaultLanguageAsync()
        {
            return await _context.Languages.AsNoTracking().FirstOrDefaultAsync(l => l.IsDefault && l.IsActive);
        }

        public async Task<Language?> GetByCodeAsync(string code)
        {
            return await _context.Languages.AsNoTracking().FirstOrDefaultAsync(l => l.Code == code);
        }

        public async Task<Language> CreateAsync(Language language)
        {
            _context.Languages.Add(language);
            await _context.SaveChangesAsync();
            return language;
        }

        public async Task UpdateAsync(Language language)
        {
            language.UpdatedAt = DateTime.UtcNow;
            _context.Languages.Update(language);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var language = await _context.Languages.FindAsync(id);
            if (language != null)
            {
                _context.Languages.Remove(language);
                await _context.SaveChangesAsync();
            }
        }
    }
}
