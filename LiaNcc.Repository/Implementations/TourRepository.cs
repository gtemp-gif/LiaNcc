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
            return await _context.Tours.AsNoTracking()
                .Include(t => t.TourCategory)
                .Include(t => t.Vehicle)
                .OrderBy(t => t.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetActiveToursAsync()
        {
            return await _context.Tours.AsNoTracking().Where(t => t.IsActive).OrderBy(t => t.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetFeaturedToursAsync()
        {
            return await _context.Tours.AsNoTracking().Where(t => t.IsActive && t.IsFeatured).OrderBy(t => t.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetByCategoryAsync(Guid categoryId)
        {
            return await _context.Tours.AsNoTracking().Where(t => t.IsActive && t.CategoryId == categoryId).OrderBy(t => t.SortOrder).ToListAsync();
        }

        public async Task<Tour?> GetByIdAsync(Guid id)
        {
            return await _context.Tours
                .Include(t => t.TourCategory)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tour?> GetBySlugAsync(string slug)
        {
            return await _context.Tours.AsNoTracking().FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task<Tour?> GetTourDetailAsync(Guid id)
        {
            return await _context.Tours.AsNoTracking()
                .Include(t => t.TourCategory)
                .Include(t => t.TourSections.OrderBy(s => s.SortOrder))
                .Include(t => t.TourGalleryImages.OrderBy(g => g.SortOrder))
                .Include(t => t.TourInfoItems.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tour?> GetTourDetailBySlugAsync(string slug)
        {
            return await _context.Tours.AsNoTracking()
                .Include(t => t.TourCategory)
                .Include(t => t.TourSections.OrderBy(s => s.SortOrder))
                .Include(t => t.TourGalleryImages.OrderBy(g => g.SortOrder))
                .Include(t => t.TourInfoItems.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync(t => t.Slug == slug);
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

        public async Task<IEnumerable<TourCategory>> GetCategoriesAsync()
        {
            return await _context.TourCategories.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync();
        }
    }
}
