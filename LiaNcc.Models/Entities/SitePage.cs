using System;
using System.Collections.Generic;

namespace LiaNcc.Models.Entities
{
    public class SitePage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<PageSection> PageSections { get; set; } = new List<PageSection>();
        public ICollection<CallToAction> CallToActions { get; set; } = new List<CallToAction>();
    }
}
