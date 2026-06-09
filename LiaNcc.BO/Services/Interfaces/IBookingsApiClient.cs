using System;
using LiaNcc.Models.Entities;
using LiaNcc.Models.DTOs.Responses;
using LiaNcc.Models.DTOs.Requests;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IBookingsApiClient
    {
        Task<IEnumerable<BookingDetailDto>> GetAllAsync(BookingFilterRequest? filter = null);
        Task<BookingDetailDto?> GetByIdAsync(Guid id);
        Task UpdateStatusAsync(Guid id, string status, string? note = null);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Service>> GetServiceTypesAsync();
    }
}
