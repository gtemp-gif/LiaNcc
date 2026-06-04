using System;
using System.Collections.Generic;

namespace LiaNcc.Models.Entities
{
    public class Tour
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? CategoryId { get; set; }
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

        public bool IsFeatured { get; set; } = false;
        public bool IsBookable { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public TourCategory? TourCategory { get; set; }
        public Vehicle? Vehicle { get; set; }
        public ICollection<TourSection> TourSections { get; set; } = new List<TourSection>();
        public ICollection<TourGalleryImage> TourGalleryImages { get; set; } = new List<TourGalleryImage>();
        public ICollection<TourInfoItem> TourInfoItems { get; set; } = new List<TourInfoItem>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
