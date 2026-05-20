using System.Collections.Generic;

namespace LiaNcc.Models.DTOs.Responses
{
    public class FileUploadResponse
    {
        public List<UploadedFileDto> UploadedFiles { get; set; } = new();
    }
}
