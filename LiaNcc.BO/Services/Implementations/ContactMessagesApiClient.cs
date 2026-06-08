using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using LiaNcc.Models.DTOs.Requests;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class ContactMessagesApiClient : BaseApiClient<ContactMessage, Guid>, IContactMessagesApiClient
    {
        public ContactMessagesApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "contactmessages") { }

        public async Task<IEnumerable<ContactMessage>> GetAllAsync(ContactMessageFilterRequest? filter = null)
        {
            SetBearerToken();
            string query = "";
            if (filter != null)
            {
                var dict = new Dictionary<string, string?>();
                if (filter.IsRead.HasValue) dict.Add("IsRead", filter.IsRead.Value.ToString());
                if (filter.FromDate.HasValue) dict.Add("FromDate", filter.FromDate.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
                if (filter.ToDate.HasValue) dict.Add("ToDate", filter.ToDate.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
                if (!string.IsNullOrEmpty(filter.SearchText)) dict.Add("SearchText", filter.SearchText);

                var queryParams = dict.Where(x => x.Value != null).Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}");
                query = "?" + string.Join("&", queryParams);
            }

            var response = await _httpClient.GetAsync(_endpointUrl + query);
            EnsureValidResponse(response);
            return await response.Content.ReadFromJsonAsync<IEnumerable<ContactMessage>>(_jsonSerializerOptions) ?? new List<ContactMessage>();
        }

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

        public async Task DeleteAsync(Guid id)
        {
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"{_endpointUrl}/{id}");
            EnsureValidResponse(response);
        }
    }
}
