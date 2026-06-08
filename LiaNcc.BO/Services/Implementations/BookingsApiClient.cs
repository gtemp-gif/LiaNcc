using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using LiaNcc.Models.DTOs.Responses;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class BookingsApiClient : BaseApiClient<Booking, Guid>, IBookingsApiClient
    {
        public BookingsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "bookings") { }

        public new async Task<IEnumerable<BookingDetailDto>> GetAllAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync(_endpointUrl);
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<IEnumerable<BookingDetailDto>>(_jsonSerializerOptions) ?? new List<BookingDetailDto>();
        }

        public new async Task<BookingDetailDto?> GetByIdAsync(Guid id)
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"{_endpointUrl}/{id}");
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<BookingDetailDto>(_jsonSerializerOptions);
        }

        public async Task UpdateStatusAsync(Guid id, string status, string? note = null)
        {
            SetBearerToken();
            var response = await _httpClient.PatchAsJsonAsync($"{_endpointUrl}/{id}/status", new { Status = status, Note = note }, _jsonSerializerOptions);
            EnsureValidResponse(response);
        }

        public new async Task DeleteAsync(Guid id)
        {
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"{_endpointUrl}/{id}");
            EnsureValidResponse(response);
        }
    }
}
