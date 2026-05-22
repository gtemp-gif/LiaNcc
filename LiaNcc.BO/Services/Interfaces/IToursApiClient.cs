using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Tours;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IToursApiClient
    {
        Task<IEnumerable<TourDto>> GetAllToursAsync();
        Task<TourDto?> GetTourByIdAsync(Guid id);
        Task<Tour> CreateTourAsync(Tour tour);
        Task UpdateTourAsync(Guid id, Tour tour);
        Task DeleteTourAsync(Guid id);
        Task<IEnumerable<TourCategory>> GetCategoriesAsync();
        Task<IEnumerable<TourGalleryImageDto>> GetTourGalleryAsync(Guid tourId);
        Task DeleteGalleryImageAsync(Guid imageId);
    }
}
