using System;
using System.Collections.Generic;

namespace LiaNcc.Models.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Seats { get; set; }
        public int Luggages { get; set; }
        public bool IsFeatured { get; set; } = false;
        public bool IsBookable { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public VehicleCategory VehicleCategory { get; set; } = null!;
        public ICollection<VehicleFeature> VehicleFeatures { get; set; } = new List<VehicleFeature>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
