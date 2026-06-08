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
            : base(httpClient, httpContextAccessor, "contactmessages") { }

        public async Task MarkAsReadAsync(Guid id)
        {
            SetBearerToken();
            var response = await _httpClient.PatchAsync($"{_endpointUrl}/{id}/read", null);
            EnsureValidResponse(response);
        }

        public async Task ReplyAsync(Guid id, string subject, string body, List<IFormFile>? attachments = null)
        {
            SetBearerToken();
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(subject), "Subject");
            content.Add(new StringContent(body), "Body");

            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "attachmentsFiles", file.FileName);
                }
            }

            var response = await _httpClient.PostAsync($"{_endpointUrl}/{id}/reply", content);
            EnsureValidResponse(response);
        }
    }
}
