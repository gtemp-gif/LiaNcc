using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync();
        Task<IEnumerable<Vehicle>> GetFeaturedVehiclesAsync();
        Task<IEnumerable<Vehicle>> GetByCategoryAsync(Guid categoryId);
        Task<Vehicle?> GetByIdAsync(Guid id);
        Task<Vehicle?> GetVehicleWithFeaturesAsync(Guid vehicleId);
        Task<Vehicle> CreateAsync(Vehicle vehicle);
        Task UpdateAsync(Vehicle vehicle);
        Task DeleteAsync(Guid id);

        Task<IEnumerable<VehicleCategory>> GetCategoriesAsync();
        Task<IEnumerable<VehicleFeature>> GetFeaturesAsync(Guid vehicleId);
    }
}
