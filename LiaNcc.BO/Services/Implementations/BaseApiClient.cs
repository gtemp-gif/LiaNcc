using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LiaNcc.BO.Services.Implementations
{
    public abstract class BaseApiClient<T, TKey> : IApiClient<T, TKey> where T : class
    {
        protected readonly HttpClient _httpClient;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly string _endpointUrl;

        protected readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public BaseApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, string endpointUrl)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _endpointUrl = endpointUrl;

            SetBearerToken();
        }

        protected void SetBearerToken()
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("jwt")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync(_endpointUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<T>>(_jsonSerializerOptions) ?? Array.Empty<T>();
        }

        public virtual async Task<T?> GetByIdAsync(TKey id)
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"{_endpointUrl}/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            SetBearerToken();
            var response = await _httpClient.PostAsJsonAsync(_endpointUrl, entity, _jsonSerializerOptions);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions))!;
        }

        public virtual async Task UpdateAsync(TKey id, T entity)
        {
            SetBearerToken();
            var response = await _httpClient.PutAsJsonAsync($"{_endpointUrl}/{id}", entity, _jsonSerializerOptions);
            response.EnsureSuccessStatusCode();
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"{_endpointUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
