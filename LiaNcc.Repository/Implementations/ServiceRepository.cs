using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly LiaNccDbContext _context;

        public ServiceRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _context.Services.AsNoTracking().OrderBy(s => s.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetActiveAsync()
        {
            return await _context.Services.AsNoTracking().Where(s => s.IsActive).OrderBy(s => s.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetFeaturedAsync()
        {
            return await _context.Services.AsNoTracking().Where(s => s.IsActive && s.IsFeatured).OrderBy(s => s.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetBookableAsync()
        {
            return await _context.Services.AsNoTracking().Where(s => s.IsActive && s.IsBookable).OrderBy(s => s.SortOrder).ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(Guid id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task<Service> CreateAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task UpdateAsync(Service service)
        {
            service.UpdatedAt = DateTime.UtcNow;
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }
    }
}
