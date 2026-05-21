using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Responses;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IFilesApiClient
    {
        Task<FileListResponse> GetFilesAsync(string? folder = null, string? search = null, string? extension = null, int page = 1, int pageSize = 20);
        Task<FileMetadataDto> GetFileMetadataAsync(string fileName, string? folder = null);
        Task<FileUploadResponse> UploadFilesAsync(List<IFormFile> files, string? folder = null, string? entityName = null, System.Guid? entityId = null, string? mediaType = null);
        Task<Stream> DownloadFileAsync(string fileName, string? folder = null);
        Task<bool> DeleteFileAsync(string fileName, string? folder = null);
    }
}
