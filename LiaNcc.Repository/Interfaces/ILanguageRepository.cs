using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface ILanguageRepository
    {
        Task<IEnumerable<Language>> GetAllAsync();
        Task<Language?> GetByIdAsync(Guid id);
        Task<Language?> GetDefaultLanguageAsync();
        Task<Language?> GetByCodeAsync(string code);
        Task<Language> CreateAsync(Language language);
        Task UpdateAsync(Language language);
        Task DeleteAsync(Guid id);
    }
}
