using System;

namespace LiaNcc.Models.Entities
{
    public class TourInfoItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TourId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Tour Tour { get; set; } = null!;
    }
}
