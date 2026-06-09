using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly LiaNccDbContext _context;

        public BookingRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync(BookingFilterRequest? filter = null)
        {
            var query = _context.Bookings
                .Include(b => b.ServiceType)
                .Include(b => b.PassengerOption)
                .Include(b => b.Tour)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v!.VehicleCategory)
                .AsNoTracking();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Status))
                    query = query.Where(b => b.Status == filter.Status);

                if (filter.ServiceTypeId.HasValue)
                    query = query.Where(b => b.ServiceTypeId == filter.ServiceTypeId);

                if (filter.TourId.HasValue)
                    query = query.Where(b => b.TourId == filter.TourId);

                if (filter.VehicleId.HasValue)
                    query = query.Where(b => b.VehicleId == filter.VehicleId);

                if (filter.FromServiceDate.HasValue)
                    query = query.Where(b => b.ServiceDate >= filter.FromServiceDate.Value);

                if (filter.ToServiceDate.HasValue)
                    query = query.Where(b => b.ServiceDate <= filter.ToServiceDate.Value);

                if (filter.FromCreatedAt.HasValue)
                    query = query.Where(b => b.CreatedAt >= filter.FromCreatedAt.Value);

                if (filter.ToCreatedAt.HasValue)
                    query = query.Where(b => b.CreatedAt <= filter.ToCreatedAt.Value);

                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    var search = filter.SearchText.ToLower();
                    query = query.Where(b => b.FullName.ToLower().Contains(search) ||
                                             b.Email.ToLower().Contains(search) ||
                                             (b.Phone != null && b.Phone.ToLower().Contains(search)) ||
                                             (b.Message != null && b.Message.ToLower().Contains(search)));
                }
            }

            return await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByStatusAsync(string status)
        {
            return await _context.Bookings
                .Include(b => b.ServiceType)
                .Include(b => b.PassengerOption)
                .Include(b => b.Tour)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v!.VehicleCategory)
                .AsNoTracking()
                .Where(b => b.Status == status)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.ServiceType)
                .Include(b => b.PassengerOption)
                .Include(b => b.Tour)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v!.VehicleCategory)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task UpdateAsync(Booking booking)
        {
            booking.UpdatedAt = DateTime.UtcNow;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(Guid id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                booking.Status = status;
                booking.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Service>> GetServiceTypesAsync()
        {
            return await _context.Services.AsNoTracking().Where(s => s.IsActive).OrderBy(s => s.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<BookingPassengerOption>> GetPassengerOptionsAsync()
        {
            return await _context.BookingPassengerOptions.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.SortOrder).ToListAsync();
        }
    }
}
