using System;
using System.Net.Http;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class SitePagesApiClient : BaseApiClient<SitePage, Guid>, ISitePagesApiClient
    {
        public SitePagesApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "sitepages") { }
    }
}
