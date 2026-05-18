using System;
using System.Collections.Generic;

namespace LiaNcc.Models.Entities
{
    public class Language
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<LocalizedContent> LocalizedContents { get; set; } = new List<LocalizedContent>();
    }
}
