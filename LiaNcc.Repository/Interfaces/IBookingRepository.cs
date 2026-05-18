using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> GetByStatusAsync(string status);
        Task<Booking?> GetByIdAsync(Guid id);
        Task<Booking> CreateAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task UpdateStatusAsync(Guid id, string status);
        Task DeleteAsync(Guid id);

        Task<IEnumerable<BookingServiceType>> GetServiceTypesAsync();
        Task<IEnumerable<BookingPassengerOption>> GetPassengerOptionsAsync();
    }
}
