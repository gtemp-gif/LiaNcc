using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface ISitePageRepository
    {
        Task<IEnumerable<SitePage>> GetAllAsync();
        Task<IEnumerable<SitePage>> GetActivePagesAsync();
        Task<SitePage?> GetByIdAsync(Guid id);
        Task<SitePage?> GetBySlugAsync(string slug);
        Task<SitePage?> GetPageWithSectionsAsync(Guid pageId);
        Task<SitePage?> GetPageWithSectionsBySlugAsync(string slug);
        Task<SitePage> CreateAsync(SitePage sitePage);
        Task UpdateAsync(SitePage sitePage);
        Task DeleteAsync(Guid id);
    }
}
