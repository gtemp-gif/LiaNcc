using System;
using System.Net.Http;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class BookingsApiClient : BaseApiClient<Booking, Guid>, IBookingsApiClient
    {
        public BookingsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "bookings") { }

        public async Task UpdateStatusAsync(Guid id, string status, string? note = null)
        {
            SetBearerToken();
            var response = await _httpClient.PatchAsJsonAsync($"{_endpointUrl}/{id}/status", new { Status = status, Note = note }, _jsonSerializerOptions);
            EnsureValidResponse(response);
        }
    }
}
