using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LiaNcc.FE.Services.Interfaces;
using LiaNcc.Models.DTOs.Dashboard;
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
        // METODO AGGIUNTO QUI DENTRO NELLA POSIZIONE CORRETTA!
        public async Task<IEnumerable<TourGalleryImageDto>> GetTourGalleryAsync(Guid id)
        {
            var url = $"tours/{id}/gallery";
            return await _httpClient.GetFromJsonAsync<IEnumerable<TourGalleryImageDto>>(url, _jsonSerializerOptions) ?? Array.Empty<TourGalleryImageDto>();
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


        public async Task<List<CompanyContactDto>> GetContactsAsync()
        {
      
                try
                {
                    var response = await _httpClient.GetAsync("company/contacts");

                    if (!response.IsSuccessStatusCode)
                    {
                        // loggare StatusCode e ReasonPhrase
                        return new List<CompanyContactDto>();
                    }

                    return await response.Content.ReadFromJsonAsync<List<CompanyContactDto>>()
                           ?? new List<CompanyContactDto>();
                }
                catch
                {
                    return new List<CompanyContactDto>();
                }
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
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ContactMessagesApiClient(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task CreateAsync(LiaNcc.Models.DTOs.Requests.ContactMessageCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("contact-messages", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<ContactMessage>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ContactMessage>>("contact-messages", _jsonSerializerOptions) ?? Array.Empty<ContactMessage>();
        }

        public async Task<ContactMessage?> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<ContactMessage>($"contact-messages/{id}", _jsonSerializerOptions);
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var response = await _httpClient.PatchAsync($"contact-messages/{id}/read", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"contact-messages/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
    public class BookingsApiClient : IBookingsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        public BookingsApiClient(HttpClient httpClient) { _httpClient = httpClient; }
        public async Task CreateAsync(LiaNcc.Models.DTOs.Requests.BookingCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("bookings", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Booking>>("bookings", _jsonSerializerOptions) ?? Array.Empty<Booking>();
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Booking>($"bookings/{id}", _jsonSerializerOptions);
        }

        public async Task UpdateStatusAsync(Guid id, string status)
        {
            var response = await _httpClient.PatchAsJsonAsync($"bookings/{id}/status", status);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<Service>> GetServiceTypesAsync(string? culture = null)
        {
            var url = string.IsNullOrEmpty(culture) ? "bookings/service-types" : $"bookings/service-types?culture={culture}";
            return await _httpClient.GetFromJsonAsync<IEnumerable<Service>>(url, _jsonSerializerOptions) ?? Array.Empty<Service>();
        }

        public async Task<IEnumerable<BookingPassengerOption>> GetPassengerOptionsAsync(string? culture = null)
        {
            var url = string.IsNullOrEmpty(culture) ? "bookings/passenger-options" : $"bookings/passenger-options?culture={culture}";
            return await _httpClient.GetFromJsonAsync<IEnumerable<BookingPassengerOption>>(url, _jsonSerializerOptions) ?? Array.Empty<BookingPassengerOption>();
        }
    }
}
