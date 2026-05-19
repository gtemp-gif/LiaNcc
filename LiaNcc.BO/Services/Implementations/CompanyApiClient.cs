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
            var profiles = await GetAllAsync();
            return profiles.FirstOrDefault();
        }
    }
}
