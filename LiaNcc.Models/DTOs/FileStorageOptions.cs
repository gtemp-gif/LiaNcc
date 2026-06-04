using System.Collections.Generic;

namespace LiaNcc.Models.DTOs
{
    public class FileStorageOptions
    {
        public string RootPath { get; set; } = "wwwroot/uploads";
        public string PublicBasePath { get; set; } = "/uploads";
        public string PublicBaseUrl { get; set; } = "";
        public int MaxFileSizeMB { get; set; } = 10;
        public List<string> AllowedExtensions { get; set; } = new() { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".pdf" };
    }
}
