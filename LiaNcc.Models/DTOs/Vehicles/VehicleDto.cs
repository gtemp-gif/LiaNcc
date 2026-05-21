using System;
using System.Collections.Generic;
using LiaNcc.Models.Entities;

namespace LiaNcc.Models.DTOs.Vehicles
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Seats { get; set; }
        public int Luggages { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBookable { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<VehicleFeatureDto> Features { get; set; } = new();
        public List<VehicleGalleryImageDto> GalleryImages { get; set; } = new();
    }

    public class VehicleFeatureDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int SortOrder { get; set; }
    }

    public class VehicleGalleryImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
