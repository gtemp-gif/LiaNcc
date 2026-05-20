using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.WebAPI.Models
{
    public class FileUploadRequest
    {
        public List<IFormFile> Files { get; set; } = new();
        public string? Folder { get; set; }
        public string? EntityName { get; set; }
        public Guid? EntityId { get; set; }
        public string? MediaType { get; set; }
    }
}
