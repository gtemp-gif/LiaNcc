using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs;
using LiaNcc.Models.DTOs.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace LiaNcc.WebAPI.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly FileStorageOptions _options;
        private readonly string _rootPath;

        public FileStorageService(IOptions<FileStorageOptions> options, IWebHostEnvironment env)
        {
            _options = options.Value;

            if (Path.IsPathRooted(_options.RootPath))
            {
                _rootPath = _options.RootPath;
            }
            else
            {
                _rootPath = Path.Combine(env.ContentRootPath, _options.RootPath);
            }

            if (!Directory.Exists(_rootPath))
            {
                Directory.CreateDirectory(_rootPath);
            }
        }

        public async Task<FileListResponse> GetFilesAsync(string? folder = null, string? search = null, string? extension = null, int page = 1, int pageSize = 20)
        {
            var targetFolder = GetSafeFolderPath(folder);
            if (!Directory.Exists(targetFolder))
            {
                return new FileListResponse();
            }

            var directoryInfo = new DirectoryInfo(targetFolder);
            var files = directoryInfo.GetFiles()
                .Select(f => MapToFileMetadata(f, folder))
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                files = files.Where(f => f.FileName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                       f.OriginalFileName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(extension))
            {
                files = files.Where(f => f.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = files.Count();
            var pagedFiles = files.OrderByDescending(f => f.CreatedAt)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            return new FileListResponse
            {
                Files = pagedFiles,
                TotalCount = totalCount
            };
        }

        public async Task<FileMetadataDto?> GetFileMetadataAsync(string fileName, string? folder = null)
        {
            var filePath = GetSafeFilePath(fileName, folder);
            if (!File.Exists(filePath)) return null;

            var fileInfo = new FileInfo(filePath);
            return MapToFileMetadata(fileInfo, folder);
        }

        public async Task<UploadedFileDto> SaveFileAsync(IFormFile file, string? folder = null)
        {
            ValidateFile(file);

            var targetFolder = GetSafeFolderPath(folder);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(targetFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = string.IsNullOrEmpty(folder)
                ? $"{_options.PublicBasePath}/{uniqueFileName}"
                : $"{_options.PublicBasePath}/{folder}/{uniqueFileName}";

            var baseUrl = _options.PublicBaseUrl?.TrimEnd('/');
            var absoluteUrl = string.IsNullOrEmpty(baseUrl)
                ? relativePath
                : $"{baseUrl}/{(string.IsNullOrEmpty(folder) ? uniqueFileName : folder + "/" + uniqueFileName)}";

            return new UploadedFileDto
            {
                FileName = uniqueFileName,
                OriginalFileName = file.FileName,
                RelativePath = relativePath,
                Url = absoluteUrl,
                Extension = extension,
                MimeType = GetMimeType(filePath),
                SizeBytes = file.Length,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteFileAsync(string fileName, string? folder = null)
        {
            var filePath = GetSafeFilePath(fileName, folder);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }

        public async Task<(Stream fileStream, string contentType, string fileName)> DownloadFileAsync(string fileName, string? folder = null)
        {
            var filePath = GetSafeFilePath(fileName, folder);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found on server.");
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
            return (stream, GetMimeType(filePath), fileName);
        }

        private string GetSafeFolderPath(string? folder)
        {
            if (string.IsNullOrWhiteSpace(folder)) return _rootPath;

            // Prevent path traversal
            var sanitizedFolder = folder.Replace("..", "").Replace("/", "").Replace("\\", "");
            return Path.Combine(_rootPath, sanitizedFolder);
        }

        private string GetSafeFilePath(string fileName, string? folder)
        {
            var targetFolder = GetSafeFolderPath(folder);
            var sanitizedFileName = Path.GetFileName(fileName);
            return Path.Combine(targetFolder, sanitizedFileName);
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_options.AllowedExtensions.Contains(extension))
                throw new ArgumentException("File extension not allowed.");

            if (file.Length > _options.MaxFileSizeMB * 1024 * 1024)
                throw new ArgumentException($"File size exceeds the limit of {_options.MaxFileSizeMB}MB.");
        }

        private FileMetadataDto MapToFileMetadata(FileInfo fileInfo, string? folder)
        {
            var relativePath = string.IsNullOrEmpty(folder)
                ? $"{_options.PublicBasePath}/{fileInfo.Name}"
                : $"{_options.PublicBasePath}/{folder}/{fileInfo.Name}";

            var baseUrl = _options.PublicBaseUrl?.TrimEnd('/');
            var absoluteUrl = string.IsNullOrEmpty(baseUrl)
                ? relativePath
                : $"{baseUrl}/{(string.IsNullOrEmpty(folder) ? fileInfo.Name : folder + "/" + fileInfo.Name)}";

            return new FileMetadataDto
            {
                FileName = fileInfo.Name,
                OriginalFileName = fileInfo.Name, // We don't store original name in FS, could be improved if needed
                RelativePath = relativePath,
                Url = absoluteUrl,
                Extension = fileInfo.Extension,
                MimeType = GetMimeType(fileInfo.FullName),
                SizeBytes = fileInfo.Length,
                CreatedAt = fileInfo.CreationTimeUtc,
                LastModifiedAt = fileInfo.LastWriteTimeUtc
            };
        }

        private string GetMimeType(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
