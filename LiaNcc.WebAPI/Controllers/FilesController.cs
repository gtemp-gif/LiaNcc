using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Responses;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using LiaNcc.WebAPI.Models;
using LiaNcc.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/files")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IMediaRepository _mediaRepository;
        private readonly ILogger<FilesController> _logger;
        private readonly IApplicationLoggerService _appLogger;

        public FilesController(
            IFileStorageService fileStorageService,
            IMediaRepository mediaRepository,
            ILogger<FilesController> logger,
            IApplicationLoggerService appLogger)
        {
            _fileStorageService = fileStorageService;
            _mediaRepository = mediaRepository;
            _logger = logger;
            _appLogger = appLogger;
        }

        [HttpGet]
        public async Task<ActionResult<FileListResponse>> GetFiles(
            [FromQuery] string? folder,
            [FromQuery] string? search,
            [FromQuery] string? extension,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var response = await _fileStorageService.GetFilesAsync(folder, search, extension, page, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing files");
                return StatusCode(500, "Internal server error while listing files");
            }
        }

        [HttpGet("{fileName}")]
        public async Task<ActionResult<FileMetadataDto>> GetFileMetadata(string fileName, [FromQuery] string? folder)
        {
            try
            {
                var metadata = await _fileStorageService.GetFileMetadataAsync(fileName, folder);
                if (metadata == null) return NotFound();
                return Ok(metadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file metadata for {FileName}", fileName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName, [FromQuery] string? folder)
        {
            try
            {
                var (stream, contentType, downloadName) = await _fileStorageService.DownloadFileAsync(fileName, folder);
                return File(stream, contentType, downloadName);
            }
            catch (System.IO.FileNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file {FileName}", fileName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<FileUploadResponse>> Upload([FromForm] FileUploadRequest request)
        {
            if (request.Files == null || request.Files.Count == 0)
            {
                return BadRequest("No files uploaded");
            }

            var response = new FileUploadResponse();

            try
            {
                foreach (var file in request.Files)
                {
                    var uploadedFile = await _fileStorageService.SaveFileAsync(file, request.Folder);

                    // Save to DB
                    var mediaAsset = new MediaAsset
                    {
                        FileName = uploadedFile.FileName,
                        FileUrl = uploadedFile.Url,
                        MimeType = uploadedFile.MimeType,
                        FileSize = (int)uploadedFile.SizeBytes,
                        CreatedAt = uploadedFile.CreatedAt
                    };

                    await _mediaRepository.CreateAsync(mediaAsset);

                    // If entity info provided, link it
                    if (!string.IsNullOrEmpty(request.EntityName) && request.EntityId.HasValue)
                    {
                        var entityMedia = new EntityMedia
                        {
                            EntityName = request.EntityName,
                            EntityId = request.EntityId.Value,
                            MediaAssetId = mediaAsset.Id,
                            MediaType = request.MediaType ?? "generic",
                            CreatedAt = DateTime.UtcNow
                        };
                        await _mediaRepository.AddMediaToEntityAsync(entityMedia);
                    }

                    response.UploadedFiles.Add(uploadedFile);
                    await _appLogger.LogInfoAsync("Media", "UploadFile", $"File {uploadedFile.FileName} uploaded to {request.Folder}", mediaAsset.Id, "MediaAsset");
                    _logger.LogInformation("File uploaded and registered: {FileName}", uploadedFile.FileName);
                }

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var fileNames = request.Files != null ? string.Join(", ", request.Files.Select(f => f.FileName)) : "none";
                var errorMsg = $"Error uploading files. Folder: {request.Folder}, Entity: {request.EntityName}, Id: {request.EntityId}, Files: [{fileNames}]";

                _logger.LogError(ex, errorMsg);
                await _appLogger.LogErrorAsync("Media", "UploadFile", errorMsg, ex, null, request.EntityId, request.EntityName);

                return StatusCode(500, $"Internal server error during upload: {ex.Message}");
            }
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName, [FromQuery] string? folder)
        {
            try
            {
                var deleted = await _fileStorageService.DeleteFileAsync(fileName, folder);
                if (!deleted) return NotFound();

                var asset = await _mediaRepository.GetByFileNameAsync(fileName);
                if (asset != null)
                {
                    await _mediaRepository.DeleteAsync(asset.Id);
                }

                _logger.LogInformation("File deleted: {FileName} from folder {Folder}", fileName, folder);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FileName}", fileName);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
