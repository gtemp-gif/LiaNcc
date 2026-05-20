using System.Collections.Generic;

namespace LiaNcc.Models.DTOs.Responses
{
    public class FileListResponse
    {
        public List<FileMetadataDto> Files { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
