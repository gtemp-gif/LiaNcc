using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllAsync();
        Task<IEnumerable<Service>> GetActiveAsync();
        Task<IEnumerable<Service>> GetFeaturedAsync();
        Task<IEnumerable<Service>> GetBookableAsync();
        Task<Service?> GetByIdAsync(Guid id);
        Task<Service> CreateAsync(Service service);
        Task UpdateAsync(Service service);
        Task DeleteAsync(Guid id);
    }
}
