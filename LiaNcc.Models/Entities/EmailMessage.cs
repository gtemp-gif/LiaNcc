using System;

namespace LiaNcc.Models.Entities
{
    public class EmailMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? RelatedEntityName { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public string ToEmail { get; set; } = string.Empty;
        public string? FromEmail { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Body { get; set; }
        public bool IsHtml { get; set; } = true;
        public string Status { get; set; } = "Pending"; // Pending, Sent, Failed
        public string? ErrorMessage { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
