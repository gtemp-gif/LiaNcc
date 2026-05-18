using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface ITourRepository
    {
        Task<IEnumerable<Tour>> GetAllAsync();
        Task<IEnumerable<Tour>> GetActiveToursAsync();
        Task<IEnumerable<Tour>> GetFeaturedToursAsync();
        Task<IEnumerable<Tour>> GetByCategoryAsync(Guid categoryId);
        Task<Tour?> GetByIdAsync(Guid id);
        Task<Tour?> GetBySlugAsync(string slug);
        Task<Tour?> GetTourDetailAsync(Guid id);
        Task<Tour?> GetTourDetailBySlugAsync(string slug);
        Task<Tour> CreateAsync(Tour tour);
        Task UpdateAsync(Tour tour);
        Task DeleteAsync(Guid id);

        Task<IEnumerable<TourCategory>> GetCategoriesAsync();
    }
}
