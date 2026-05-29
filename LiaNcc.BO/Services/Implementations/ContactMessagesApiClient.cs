using System;
using System.Net.Http;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class ContactMessagesApiClient : BaseApiClient<ContactMessage, Guid>, IContactMessagesApiClient
    {
        public ContactMessagesApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "contact-messages") { }

        public async Task MarkAsReadAsync(Guid id)
        {
            SetBearerToken();
            var response = await _httpClient.PatchAsync($"{_endpointUrl}/{id}/read", null);
            EnsureValidResponse(response);
        }
    }
}
