using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface ILocalizedContentRepository
    {
        Task<IEnumerable<LocalizedContent>> GetAllAsync();
        Task<LocalizedContent?> GetByIdAsync(Guid id);
        Task<IEnumerable<LocalizedContent>> GetByEntityAsync(string entityName, Guid entityId, string languageCode);
        Task<IEnumerable<LocalizedContent>> GetByEntityAsync(string entityName, Guid entityId);
        Task<LocalizedContent?> GetStaticContentAsync(string contentKey, string languageCode);
        Task<IEnumerable<LocalizedContent>> GetStaticContentsAsync(string languageCode);
        Task<LocalizedContent> CreateAsync(LocalizedContent localizedContent);
        Task UpdateAsync(LocalizedContent localizedContent);
        Task UpsertBatchAsync(IEnumerable<LocalizedContent> items);
        Task DeleteAsync(Guid id);
    }
}
