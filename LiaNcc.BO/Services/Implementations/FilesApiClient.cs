using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.DTOs.Responses;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Implementations
{
    public class FilesApiClient : BaseApiClient<object, Guid>, IFilesApiClient
    {
        public FilesApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor, "files")
        {
        }

        public async Task<FileListResponse> GetFilesAsync(string? folder = null, string? search = null, string? extension = null, int page = 1, int pageSize = 20)
        {
            SetBearerToken();
            var query = $"?folder={Uri.EscapeDataString(folder ?? "")}&search={Uri.EscapeDataString(search ?? "")}&extension={Uri.EscapeDataString(extension ?? "")}&page={page}&pageSize={pageSize}";
            var response = await _httpClient.GetFromJsonAsync<FileListResponse>($"{_endpointUrl}{query}", _jsonSerializerOptions);
            return response ?? new FileListResponse();
        }

        public async Task<FileMetadataDto> GetFileMetadataAsync(string fileName, string? folder = null)
        {
            SetBearerToken();
            var query = $"?folder={Uri.EscapeDataString(folder ?? "")}";
            var response = await _httpClient.GetFromJsonAsync<FileMetadataDto>($"{_endpointUrl}/{Uri.EscapeDataString(fileName)}{query}", _jsonSerializerOptions);
            return response!;
        }

        public async Task<FileUploadResponse> UploadFilesAsync(List<IFormFile> files, string? folder = null, string? entityName = null, Guid? entityId = null, string? mediaType = null)
        {
            SetBearerToken();
            using var content = new MultipartFormDataContent();

            if (folder != null) content.Add(new StringContent(folder), "Folder");
            if (entityName != null) content.Add(new StringContent(entityName), "EntityName");
            if (entityId != null) content.Add(new StringContent(entityId.ToString()!), "EntityId");
            if (mediaType != null) content.Add(new StringContent(mediaType), "MediaType");

            var fileNames = string.Join(", ", files.Select(f => f.FileName));

            foreach (var file in files)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "Files", file.FileName);
            }

            var response = await _httpClient.PostAsync($"{_endpointUrl}/upload", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var errorMessage = $"Upload file failed. StatusCode: {response.StatusCode}. Body: {errorBody}. " +
                                  $"Details: Folder={folder}, Entity={entityName}, Id={entityId}, Type={mediaType}, Files=[{fileNames}]";

                throw new Exception(errorMessage);
            }

            return (await response.Content.ReadFromJsonAsync<FileUploadResponse>(_jsonSerializerOptions))!;
        }

        public async Task<Stream> DownloadFileAsync(string fileName, string? folder = null)
        {
            SetBearerToken();
            var query = $"?folder={Uri.EscapeDataString(folder ?? "")}";
            var response = await _httpClient.GetAsync($"{_endpointUrl}/download/{Uri.EscapeDataString(fileName)}{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<bool> DeleteFileAsync(string fileName, string? folder = null)
        {
            SetBearerToken();
            var query = $"?folder={Uri.EscapeDataString(folder ?? "")}";
            var response = await _httpClient.DeleteAsync($"{_endpointUrl}/{Uri.EscapeDataString(fileName)}{query}");
            return response.IsSuccessStatusCode;
        }
    }
}
