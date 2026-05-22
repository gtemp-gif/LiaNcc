using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LiaNcc.FE.Services.Interfaces;
using LiaNcc.Models.DTOs.Tours;
using LiaNcc.Models.DTOs.Vehicles;
using LiaNcc.Models.Entities;

namespace LiaNcc.FE.Services.Implementations
{
    public class ServicesApiClient : BaseApiClient<Service, Guid>, IServicesApiClient
    {
        public ServicesApiClient(HttpClient httpClient) : base(httpClient, "services") { }

        public async Task<IEnumerable<Service>> GetActiveAsync(string? culture = null)
        {
            var url = string.IsNullOrEmpty(culture) ? "services/active" : $"services/active?culture={culture}";
            return await _httpClient.GetFromJsonAsync<IEnumerable<Service>>(url, _jsonSerializerOptions) ?? Array.Empty<Service>();
        }

        public async Task<IEnumerable<Service>> GetFeaturedAsync(string? culture = null)
        {
            var url = string.IsNullOrEmpty(culture) ? "services/featured" : $"services/featured?culture={culture}";
            return await _httpClient.GetFromJsonAsync<IEnumerable<Service>>(url, _jsonSerializerOptions) ?? Array.Empty<Service>();
        }
    }

    public class VehiclesApiClient : BaseApiClient<VehicleDto, Guid>, IVehiclesApiClient
    {
        public VehiclesApiClient(HttpClient httpClient) : base(httpClient, "vehicles") { }

        public async Task<IEnumerable<VehicleDto>> GetActiveAsync(string? culture = null)
        {
            var url = string.IsNullOrEmpty(culture) ? "vehicles/active" : $"vehicles/active?culture={culture}";
            return await _httpClient.GetFromJsonAsync<IEnumerable<VehicleDto>>(url, _jsonSerializerOptions) ?? Array.Empty<VehicleDto>();
        }

        public async Task<IEnumerable<VehicleDto>> GetFeaturedAsync(string? culture = null)
        {
            var url = string.IsNullOrEmpty(culture) ? "vehicles/featured" : $"vehicles/featured?culture={culture}";
            return await _httpClient.GetFromJsonAsync<IEnumerable<VehicleDto>>(url, _jsonSerializerOptions) ?? Array.Empty<VehicleDto>();
        }
    }

    public class ToursApiClient : BaseApiClient<TourDto, Guid>, IToursApiClient
    {
        public ToursApiClient(HttpClient httpClient) : base(httpClient, "tours") { }

        public async Task<IEnumerable<TourDto>> GetActiveAsync(string? culture = null)
        {
            var url = string.IsNullOrEmpty(culture) ? "tours/active" : $"tours/active?culture={culture}";
            return await _httpClient.GetFromJsonAsync<IEnumerable<TourDto>>(url, _jsonSerializerOptions) ?? Array.Empty<TourDto>();
        }

        public async Task<IEnumerable<TourDto>> GetFeaturedAsync(string? culture = null)
        {
            var url = string.IsNullOrEmpty(culture) ? "tours/featured" : $"tours/featured?culture={culture}";
            return await _httpClient.GetFromJsonAsync<IEnumerable<TourDto>>(url, _jsonSerializerOptions) ?? Array.Empty<TourDto>();
        }

        public async Task<Tour?> GetDetailAsync(Guid id, string? culture = null)
        {
            var url = $"tours/{id}/detail";
            if (!string.IsNullOrEmpty(culture)) url += $"?culture={culture}";
            return await _httpClient.GetFromJsonAsync<Tour>(url, _jsonSerializerOptions);
        }

        public async Task<Tour?> GetDetailBySlugAsync(string slug, string? culture = null)
        {
            var url = $"tours/slug/{slug}/detail";
            if (!string.IsNullOrEmpty(culture)) url += $"?culture={culture}";
            return await _httpClient.GetFromJsonAsync<Tour>(url, _jsonSerializerOptions);
        }
    }

    public class PartnersApiClient : BaseApiClient<Partner, Guid>, IPartnersApiClient
    {
        public PartnersApiClient(HttpClient httpClient) : base(httpClient, "partners") { }

        public async Task<IEnumerable<Partner>> GetActiveAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Partner>>("partners/active", _jsonSerializerOptions) ?? Array.Empty<Partner>();
        }
    }

    public class CompanyApiClient : ICompanyApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public CompanyApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CompanyProfile?> GetCompanyProfileAsync()
        {
            return await _httpClient.GetFromJsonAsync<CompanyProfile>("company", _jsonSerializerOptions);
        }

        public async Task<IEnumerable<CompanyContact>> GetContactsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CompanyContact>>("company/contacts", _jsonSerializerOptions) ?? Array.Empty<CompanyContact>();
        }
    }

    public class SitePagesApiClient : BaseApiClient<SitePage, Guid>, ISitePagesApiClient
    {
        public SitePagesApiClient(HttpClient httpClient) : base(httpClient, "site-pages") { }

        public async Task<SitePage?> GetBySlugAsync(string slug, string? culture = null)
        {
            var url = $"site-pages/slug/{slug}";
            if (!string.IsNullOrEmpty(culture)) url += $"?culture={culture}";
            return await _httpClient.GetFromJsonAsync<SitePage>(url, _jsonSerializerOptions);
        }

        public async Task<SitePage?> GetFullBySlugAsync(string slug, string? culture = null)
        {
            var url = $"site-pages/slug/{slug}/full";
            if (!string.IsNullOrEmpty(culture)) url += $"?culture={culture}";
            return await _httpClient.GetFromJsonAsync<SitePage>(url, _jsonSerializerOptions);
        }
    }

    public class ContactMessagesApiClient : IContactMessagesApiClient
    {
        private readonly HttpClient _httpClient;
        public ContactMessagesApiClient(HttpClient httpClient) { _httpClient = httpClient; }
        public async Task CreateAsync(ContactMessage message)
        {
            var response = await _httpClient.PostAsJsonAsync("contactmessages", message);
            response.EnsureSuccessStatusCode();
        }
    }

    public class BookingsApiClient : IBookingsApiClient
    {
        private readonly HttpClient _httpClient;
        public BookingsApiClient(HttpClient httpClient) { _httpClient = httpClient; }
        public async Task CreateAsync(Booking booking)
        {
            var response = await _httpClient.PostAsJsonAsync("bookings", booking);
            response.EnsureSuccessStatusCode();
        }
    }
}
