using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface ICallToActionRepository
    {
        Task<IEnumerable<CallToAction>> GetAllAsync();
        Task<IEnumerable<CallToAction>> GetActiveAsync();
        Task<CallToAction?> GetByIdAsync(Guid id);
        Task<IEnumerable<CallToAction>> GetByPageAsync(Guid pageId);
        Task<IEnumerable<CallToAction>> GetBySectionAsync(Guid sectionId);
        Task<CallToAction> CreateAsync(CallToAction cta);
        Task UpdateAsync(CallToAction cta);
        Task DeleteAsync(Guid id);
    }
}
