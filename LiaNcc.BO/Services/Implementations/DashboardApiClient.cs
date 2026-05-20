using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.DTOs.Dashboard;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class DashboardApiClient : BaseApiClient<DashboardSummaryDto, Guid>, IDashboardApiClient
    {
        public DashboardApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "dashboard") { }

        public async Task<DashboardSummaryDto?> GetSummaryAsync()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"{_endpointUrl}/summary");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    httpContext.Response.Redirect("/Auth/Login");
                    await httpContext.Response.CompleteAsync();
                    return null;
                }
            }
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<DashboardSummaryDto>(_jsonSerializerOptions);
        }
    }
}
