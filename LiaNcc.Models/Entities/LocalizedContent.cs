using System;

namespace LiaNcc.Models.Entities
{
    public class LocalizedContent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntityName { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string ContentKey { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
        public string? ContentValue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Language? Language { get; set; }
    }
}
