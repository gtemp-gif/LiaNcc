using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class LocalizedContentsApiClient : BaseApiClient<LocalizedContent, Guid>, ILocalizedContentsApiClient
    {
        public LocalizedContentsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "localization") { }

        public async Task<IEnumerable<LocalizedContent>> GetByEntityAsync(string entityName, Guid entityId)
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"{_endpointUrl}/entity/{entityName}/{entityId}");
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<IEnumerable<LocalizedContent>>(_jsonSerializerOptions) ?? new List<LocalizedContent>();
        }

        public async Task UpsertBatchAsync(IEnumerable<LocalizedContent> items)
        {
            SetBearerToken();
            var response = await _httpClient.PostAsJsonAsync($"{_endpointUrl}/upsert-batch", items, _jsonSerializerOptions);
            EnsureValidResponse(response);
        }
    }
}
