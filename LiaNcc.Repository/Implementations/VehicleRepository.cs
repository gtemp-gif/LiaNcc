using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly LiaNccDbContext _context;

        public VehicleRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles.AsNoTracking().OrderBy(v => v.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync()
        {
            return await _context.Vehicles.AsNoTracking().Where(v => v.IsActive).OrderBy(v => v.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetFeaturedVehiclesAsync()
        {
            return await _context.Vehicles.AsNoTracking().Where(v => v.IsActive && v.IsFeatured).OrderBy(v => v.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetByCategoryAsync(Guid categoryId)
        {
            return await _context.Vehicles.AsNoTracking().Where(v => v.IsActive && v.CategoryId == categoryId).OrderBy(v => v.SortOrder).ToListAsync();
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<Vehicle?> GetVehicleWithFeaturesAsync(Guid vehicleId)
        {
            return await _context.Vehicles.AsNoTracking()
                .Include(v => v.VehicleCategory)
                .Include(v => v.VehicleFeatures.OrderBy(f => f.SortOrder))
                .FirstOrDefaultAsync(v => v.Id == vehicleId);
        }

        public async Task<Vehicle> CreateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            vehicle.UpdatedAt = DateTime.UtcNow;
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<VehicleCategory>> GetCategoriesAsync()
        {
            return await _context.VehicleCategories.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<VehicleCategory>> GetActiveCategoriesAsync()
        {
            return await _context.VehicleCategories.AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehicleFeature>> GetFeaturesAsync(Guid vehicleId)
        {
            return await _context.VehicleFeatures.AsNoTracking().Where(f => f.VehicleId == vehicleId).OrderBy(f => f.SortOrder).ToListAsync();
        }

        public async Task AddFeatureAsync(VehicleFeature feature)
        {
            _context.VehicleFeatures.Add(feature);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFeatureAsync(Guid featureId)
        {
            var feature = await _context.VehicleFeatures.FindAsync(featureId);
            if (feature != null)
            {
                _context.VehicleFeatures.Remove(feature);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearFeaturesAsync(Guid vehicleId)
        {
            var features = await _context.VehicleFeatures.Where(f => f.VehicleId == vehicleId).ToListAsync();
            _context.VehicleFeatures.RemoveRange(features);
            await _context.SaveChangesAsync();
        }
    }
}
