using System.Collections.Generic;

namespace LiaNcc.Models.DTOs.Requests
{
    public class ReplyMessageRequest
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        // Attachments will be handled via IFormFile in the Controller directly or by a more specific DTO in the WebAPI project if needed,
        // but for now we keep it simple to avoid dependency issues in LiaNcc.Models
    }
}
