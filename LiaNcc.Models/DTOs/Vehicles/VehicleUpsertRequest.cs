using System;
using System.Collections.Generic;

namespace LiaNcc.Models.DTOs.Vehicles
{
    public class VehicleUpsertRequest
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Seats { get; set; }
        public int Luggages { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBookable { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }

        public List<VehicleFeatureDto> Features { get; set; } = new();
        // Gallery images are usually handled separately via file upload and then association
    }
}
