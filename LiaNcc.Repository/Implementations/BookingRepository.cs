using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
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

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings.AsNoTracking().OrderByDescending(b => b.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByStatusAsync(string status)
        {
            return await _context.Bookings.AsNoTracking().Where(b => b.Status == status).OrderByDescending(b => b.CreatedAt).ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings.FindAsync(id);
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

        public async Task<IEnumerable<BookingServiceType>> GetServiceTypesAsync()
        {
            return await _context.BookingServiceTypes.AsNoTracking().Where(s => s.IsActive).OrderBy(s => s.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<BookingPassengerOption>> GetPassengerOptionsAsync()
        {
            return await _context.BookingPassengerOptions.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.SortOrder).ToListAsync();
        }
    }
}
