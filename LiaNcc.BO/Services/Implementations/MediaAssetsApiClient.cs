using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace LiaNcc.BO.Services.Implementations
{
    public class MediaAssetsApiClient : BaseApiClient<MediaAsset, Guid>, IMediaAssetsApiClient
    {
        public MediaAssetsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "media")
        {
        }

        public async Task<MediaAsset> UploadMediaAsync(IFormFile file)
        {
            SetBearerToken();
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "file", file.FileName);

            var response = await _httpClient.PostAsync($"{_endpointUrl}/upload", content);
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<MediaAsset>(_jsonSerializerOptions))!;
        }
    }
}
