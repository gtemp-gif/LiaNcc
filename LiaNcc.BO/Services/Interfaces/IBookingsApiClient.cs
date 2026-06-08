using System;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IBookingsApiClient : IApiClient<Booking, Guid>
    {
        Task UpdateStatusAsync(Guid id, string status, string? note = null);
    }
}
