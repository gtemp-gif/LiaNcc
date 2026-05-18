using System;

namespace LiaNcc.Models.Entities
{
    public class VehicleFeature
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VehicleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Vehicle Vehicle { get; set; } = null!;
    }
}
