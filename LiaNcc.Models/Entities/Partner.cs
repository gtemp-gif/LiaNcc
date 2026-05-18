using System;

namespace LiaNcc.Models.Entities
{
    public class Partner
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? WebsiteUrl { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
