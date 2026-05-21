using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LiaNcc.FE.Services.Interfaces;

namespace LiaNcc.FE.Services.Implementations
{
    public abstract class BaseApiClient<T, TKey> : IApiClient<T, TKey> where T : class
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _endpointUrl;

        protected readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public BaseApiClient(HttpClient httpClient, string endpointUrl)
        {
            _httpClient = httpClient;
            _endpointUrl = endpointUrl;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(string? culture = null)
        {
            var url = string.IsNullOrEmpty(culture) ? _endpointUrl : $"{_endpointUrl}?culture={culture}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<T>>(_jsonSerializerOptions) ?? Array.Empty<T>();
        }

        public virtual async Task<T?> GetByIdAsync(TKey id, string? culture = null)
        {
            var url = $"{_endpointUrl}/{id}";
            if (!string.IsNullOrEmpty(culture)) url += $"?culture={culture}";

            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions);
        }
    }
}
