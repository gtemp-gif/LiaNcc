using System;
using System.Collections.Generic;

namespace LiaNcc.Models.DTOs.Tours
{
    public class TourDto
    {
        public Guid Id { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
        public string? HeroTitle { get; set; }
        public string? HeroSubtitle { get; set; }
        public string? ExperienceDescription { get; set; }
        public string? ExperienceImageUrl { get; set; }
        public int? DurationDays { get; set; }
        public int? DurationHours { get; set; }
        public string? MeetingPoint { get; set; }
        public Guid? VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBookable { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<TourGalleryImageDto> GalleryImages { get; set; } = new();
    }

    public class TourGalleryImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
