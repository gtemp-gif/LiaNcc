using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Responses;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.WebAPI.Services
{
    public interface IFileStorageService
    {
        Task<FileListResponse> GetFilesAsync(string? folder = null, string? search = null, string? extension = null, int page = 1, int pageSize = 20);
        Task<FileMetadataDto?> GetFileMetadataAsync(string fileName, string? folder = null);
        Task<UploadedFileDto> SaveFileAsync(IFormFile file, string? folder = null);
        Task<bool> DeleteFileAsync(string fileName, string? folder = null);
        Task<(Stream fileStream, string contentType, string fileName)> DownloadFileAsync(string fileName, string? folder = null);
    }
}
