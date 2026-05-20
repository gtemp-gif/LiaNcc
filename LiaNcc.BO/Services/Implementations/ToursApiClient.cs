using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.DTOs.Tours;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class ToursApiClient : BaseApiClient<Tour, Guid>, IToursApiClient
    {
        public ToursApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "tours")
        {
        }

        public async Task<IEnumerable<TourDto>> GetAllToursAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<TourDto>>(_endpointUrl, _jsonSerializerOptions);
            return response ?? new List<TourDto>();
        }

        public async Task<TourDto?> GetTourByIdAsync(Guid id)
        {
            SetBearerToken();
            try
            {
                return await _httpClient.GetFromJsonAsync<TourDto>($"{_endpointUrl}/{id}", _jsonSerializerOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task<Tour> CreateTourAsync(Tour tour)
        {
            SetBearerToken();
            var response = await _httpClient.PostAsJsonAsync(_endpointUrl, tour, _jsonSerializerOptions);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Tour>(_jsonSerializerOptions))!;
        }

        public async Task UpdateTourAsync(Guid id, Tour tour)
        {
            SetBearerToken();
            var response = await _httpClient.PutAsJsonAsync($"{_endpointUrl}/{id}", tour, _jsonSerializerOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTourAsync(Guid id)
        {
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"{_endpointUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TourCategory>> GetCategoriesAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<TourCategory>>($"{_endpointUrl}/categories", _jsonSerializerOptions);
            return response ?? new List<TourCategory>();
        }
    }
}
