using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.DTOs.Vehicles;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class VehiclesApiClient : BaseApiClient<Vehicle, Guid>, IVehiclesApiClient
    {
        public VehiclesApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "vehicles")
        {
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<VehicleDto>>(_endpointUrl, _jsonSerializerOptions);
            return response ?? new List<VehicleDto>();
        }

        public async Task<VehicleDto?> GetVehicleByIdAsync(Guid id)
        {
            SetBearerToken();
            try
            {
                return await _httpClient.GetFromJsonAsync<VehicleDto>($"{_endpointUrl}/{id}", _jsonSerializerOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task<Vehicle> CreateVehicleAsync(VehicleUpsertRequest request)
        {
            SetBearerToken();
            var response = await _httpClient.PostAsJsonAsync(_endpointUrl, request, _jsonSerializerOptions);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Vehicle>(_jsonSerializerOptions))!;
        }

        public async Task UpdateVehicleAsync(Guid id, VehicleUpsertRequest request)
        {
            SetBearerToken();
            var response = await _httpClient.PutAsJsonAsync($"{_endpointUrl}/{id}", request, _jsonSerializerOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteVehicleAsync(Guid id)
        {
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"{_endpointUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<VehicleCategory>> GetCategoriesAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<VehicleCategory>>($"{_endpointUrl}/categories", _jsonSerializerOptions);
            return response ?? new List<VehicleCategory>();
        }

        public async Task<IEnumerable<VehicleCategory>> GetActiveCategoriesAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<VehicleCategory>>($"{_endpointUrl}/categories/active", _jsonSerializerOptions);
            return response ?? new List<VehicleCategory>();
        }

        public async Task<IEnumerable<VehicleGalleryImageDto>> GetVehicleGalleryAsync(Guid vehicleId)
        {
            SetBearerToken();
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<VehicleGalleryImageDto>>($"{_endpointUrl}/{vehicleId}/gallery", _jsonSerializerOptions);
            return response ?? new List<VehicleGalleryImageDto>();
        }

        public async Task DeleteGalleryImageAsync(Guid imageId)
        {
            // Note: If using EntityMedia, we might need a dedicated MediaApiClient or similar
            // For now let's assume there is a way to delete media from vehicle
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"{_endpointUrl}/gallery/{imageId}");
            // If this endpoint doesn't exist yet, we'll need to add it or use Media controller
        }
    }
}
