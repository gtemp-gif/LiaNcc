using LiaNcc.Models.Entities;
using LiaNcc.Models.DTOs.Tours;
using LiaNcc.Models.DTOs.Vehicles;

namespace LiaNcc.FE.Services
{
    public interface ILiaNccApiClient
    {
        Task<SitePage?> GetSitePageBySlugAsync(string slug, string? culture = null);
        Task<IEnumerable<Service>> GetServicesAsync(string? culture = null);
        Task<IEnumerable<Vehicle>> GetVehiclesAsync(string? culture = null);
        Task<IEnumerable<Tour>> GetToursAsync(string? culture = null);
        Task<Tour?> GetTourBySlugAsync(string slug, string? culture = null);
        Task<IEnumerable<Partner>> GetPartnersAsync(bool activeOnly = true);
        Task<CompanyProfile?> GetCompanyProfileAsync(string? culture = null);
    }

    public class LiaNccApiClient : ILiaNccApiClient
    {
        private readonly HttpClient _httpClient;

        public LiaNccApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SitePage?> GetSitePageBySlugAsync(string slug, string? culture = null)
        {
            var url = $"site-pages/slug/{slug}/full";
            if (!string.IsNullOrEmpty(culture))
            {
                url += $"?culture={culture}";
            }
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SitePage>();
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public async Task<IEnumerable<Service>> GetServicesAsync(string? culture = null)
        {
            var url = "services";
            if (!string.IsNullOrEmpty(culture))
            {
                url += $"?culture={culture}";
            }
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<Service>>();
                    if (result != null) return result;
                }
            }
            catch (Exception)
            {
            }
            return new List<Service>();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesAsync(string? culture = null)
        {
            var url = "vehicles";
            if (!string.IsNullOrEmpty(culture))
            {
                url += $"?culture={culture}";
            }
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<Vehicle>>();
                    if (result != null) return result;
                }
            }
            catch (Exception)
            {
            }
            return new List<Vehicle>();
        }

        public async Task<IEnumerable<Tour>> GetToursAsync(string? culture = null)
        {
            var url = "tours";
            if (!string.IsNullOrEmpty(culture))
            {
                url += $"?culture={culture}";
            }
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<Tour>>();
                    if (result != null) return result;
                }
            }
            catch (Exception)
            {
            }
            return new List<Tour>();
        }

        public async Task<Tour?> GetTourBySlugAsync(string slug, string? culture = null)
        {
            var url = $"tours/slug/{slug}/detail";
            if (!string.IsNullOrEmpty(culture))
            {
                url += $"?culture={culture}";
            }
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Tour>();
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public async Task<IEnumerable<Partner>> GetPartnersAsync(bool activeOnly = true)
        {
            var url = $"partners?activeOnly={activeOnly.ToString().ToLower()}";
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<Partner>>();
                    if (result != null) return result;
                }
            }
            catch (Exception)
            {
            }
            return new List<Partner>();
        }

        public async Task<CompanyProfile?> GetCompanyProfileAsync(string? culture = null)
        {
            var url = "company";
            if (!string.IsNullOrEmpty(culture))
            {
                url += $"?culture={culture}";
            }
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CompanyProfile>();
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
    }
}
