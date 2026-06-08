using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using LiaNcc.Models.DTOs.Responses;
using LiaNcc.Models.DTOs.Requests;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class BookingsApiClient : BaseApiClient<Booking, Guid>, IBookingsApiClient
    {
        public BookingsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "bookings") { }

        public async Task<IEnumerable<BookingDetailDto>> GetAllAsync(BookingFilterRequest? filter = null)
        {
            SetBearerToken();
            string query = "";
            if (filter != null)
            {
                var dict = new Dictionary<string, string?>();
                if (!string.IsNullOrEmpty(filter.Status)) dict.Add("Status", filter.Status);
                if (filter.ServiceTypeId.HasValue) dict.Add("ServiceTypeId", filter.ServiceTypeId.Value.ToString());
                if (filter.TourId.HasValue) dict.Add("TourId", filter.TourId.Value.ToString());
                if (filter.VehicleId.HasValue) dict.Add("VehicleId", filter.VehicleId.Value.ToString());
                if (filter.FromServiceDate.HasValue) dict.Add("FromServiceDate", filter.FromServiceDate.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
                if (filter.ToServiceDate.HasValue) dict.Add("ToServiceDate", filter.ToServiceDate.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
                if (filter.FromCreatedAt.HasValue) dict.Add("FromCreatedAt", filter.FromCreatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
                if (filter.ToCreatedAt.HasValue) dict.Add("ToCreatedAt", filter.ToCreatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
                if (!string.IsNullOrEmpty(filter.SearchText)) dict.Add("SearchText", filter.SearchText);

                var queryParams = dict.Where(x => x.Value != null).Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}");
                query = "?" + string.Join("&", queryParams);
            }

            var response = await _httpClient.GetAsync(_endpointUrl + query);
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<IEnumerable<BookingDetailDto>>(_jsonSerializerOptions) ?? new List<BookingDetailDto>();
        }

        public async Task<BookingDetailDto?> GetByIdAsync(Guid id)
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

        public async Task DeleteAsync(Guid id)
        {
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"{_endpointUrl}/{id}");
            EnsureValidResponse(response);
        }

        public async Task<IEnumerable<BookingServiceType>> GetServiceTypesAsync()
        {
            var response = await _httpClient.GetAsync($"{_endpointUrl}/service-types");
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<IEnumerable<BookingServiceType>>(_jsonSerializerOptions) ?? new List<BookingServiceType>();
        }
    }
}
