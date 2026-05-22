using System;

namespace LiaNcc.Models.Entities
{
    public class CompanyContact
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string Type { get; set; } = string.Empty; // e.g., 'Email', 'Phone'
        public string Value { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public CompanyProfile? CompanyProfile { get; set; }
    }
}
