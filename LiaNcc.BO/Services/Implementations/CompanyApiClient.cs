using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class CompanyApiClient : BaseApiClient<CompanyProfile, Guid>, ICompanyApiClient
    {
        public CompanyApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "company") { }

        public async Task<CompanyProfile?> GetFirstCompanyProfileAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync(_endpointUrl);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<CompanyProfile>(_jsonSerializerOptions);
        }

        public override async Task UpdateAsync(Guid id, CompanyProfile entity)
        {
            SetBearerToken();
            var response = await _httpClient.PutAsJsonAsync(_endpointUrl, entity, _jsonSerializerOptions);
            EnsureValidResponse(response);
        }
    }
}
