using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface ITourRepository
    {
        Task<IEnumerable<Tour>> GetAllAsync();
        Task<IEnumerable<Tour>> GetActiveAsync();
        Task<Tour?> GetByIdAsync(Guid id);
        Task<Tour> CreateAsync(Tour tour);
        Task UpdateAsync(Tour tour);
        Task DeleteAsync(Guid id);
    }
}
