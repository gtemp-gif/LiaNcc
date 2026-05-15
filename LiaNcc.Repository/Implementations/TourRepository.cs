using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class TourRepository : ITourRepository
    {
        private readonly LiaNccDbContext _context;

        public TourRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tour>> GetAllAsync()
        {
            return await _context.Tours.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetActiveAsync()
        {
            return await _context.Tours.AsNoTracking()
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        public async Task<Tour?> GetByIdAsync(Guid id)
        {
            return await _context.Tours.FindAsync(id);
        }

        public async Task<Tour> CreateAsync(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
            return tour;
        }

        public async Task UpdateAsync(Tour tour)
        {
            tour.UpdatedAt = DateTime.UtcNow;
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour != null)
            {
                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
            }
        }
    }
}
