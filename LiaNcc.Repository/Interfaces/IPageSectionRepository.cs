using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface IPageSectionRepository
    {
        Task<IEnumerable<PageSection>> GetAllAsync();
        Task<IEnumerable<PageSection>> GetActiveSectionsAsync();
        Task<PageSection?> GetByIdAsync(Guid id);
        Task<IEnumerable<PageSection>> GetSectionsByPageAsync(Guid pageId);
        Task<PageSection> CreateAsync(PageSection pageSection);
        Task UpdateAsync(PageSection pageSection);
        Task DeleteAsync(Guid id);
    }
}
