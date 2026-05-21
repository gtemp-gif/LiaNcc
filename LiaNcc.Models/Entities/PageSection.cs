using System;
using System.Collections.Generic;

namespace LiaNcc.Models.Entities
{
    public class PageSection
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public SitePage SitePage { get; set; } = null!;
        public ICollection<CallToAction> CallToActions { get; set; } = new List<CallToAction>();
    }
}
