using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Vehicles;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IVehiclesApiClient
    {
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
        Task<VehicleDto?> GetVehicleByIdAsync(Guid id);
        Task<Vehicle> CreateVehicleAsync(VehicleUpsertRequest request);
        Task UpdateVehicleAsync(Guid id, VehicleUpsertRequest request);
        Task DeleteVehicleAsync(Guid id);

        Task<IEnumerable<VehicleCategory>> GetCategoriesAsync();
        Task<IEnumerable<VehicleCategory>> GetActiveCategoriesAsync();

        Task<IEnumerable<VehicleGalleryImageDto>> GetVehicleGalleryAsync(Guid vehicleId);
        Task DeleteGalleryImageAsync(Guid imageId);
    }
}
