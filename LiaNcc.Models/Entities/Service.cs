using System;

namespace LiaNcc.Models.Entities
{
    public class Service
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public bool IsFeatured { get; set; } = false;
        public bool IsBookable { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public string? CoverImageUrl { get; set; }
        public Guid? CoverImageMediaId { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
