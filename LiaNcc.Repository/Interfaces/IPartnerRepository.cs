using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface IPartnerRepository
    {
        Task<IEnumerable<Partner>> GetAllAsync();
        Task<IEnumerable<Partner>> GetActivePartnersAsync();
        Task<Partner?> GetByIdAsync(Guid id);
        Task<Partner> CreateAsync(Partner partner);
        Task UpdateAsync(Partner partner);
        Task DeleteAsync(Guid id);
    }
}
