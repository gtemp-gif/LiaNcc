using System;

namespace LiaNcc.Models.Entities
{
    public class TourGalleryImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TourId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Tour Tour { get; set; } = null!;
    }
}
