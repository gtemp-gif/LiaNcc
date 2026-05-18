using System;

namespace LiaNcc.Models.Entities
{
    public class TourSection
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TourId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Tour Tour { get; set; } = null!;
    }
}
