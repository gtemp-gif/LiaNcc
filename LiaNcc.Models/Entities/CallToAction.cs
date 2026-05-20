using System;

namespace LiaNcc.Models.Entities
{
    public class CallToAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? PageId { get; set; }
        public Guid? SectionId { get; set; }
        public string? Label { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Action { get; set; }
        public string? Controller { get; set; }
        public string? VisualStyle { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public SitePage? SitePage { get; set; }
        public PageSection? PageSection { get; set; }
    }
}
