using System;
using System.Collections.Generic;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LiaNcc.BO.Models.Tours
{
    public class TourViewModel
    {
        public Guid? Id { get; set; }
        public Guid? CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public decimal? Price { get; set; }

        public string? CoverImageUrl { get; set; }
        public IFormFile? CoverImageFile { get; set; }

        public string? Description { get; set; }
        public string? HeroTitle { get; set; }
        public string? HeroSubtitle { get; set; }

        public string? ExperienceDescription { get; set; }
        public string? ExperienceImageUrl { get; set; }
        public IFormFile? ExperienceImageFile { get; set; }

        public int? DurationDays { get; set; }
        public int? DurationHours { get; set; }
        public string? MeetingPoint { get; set; }
        public Guid? VehicleId { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }

        public IEnumerable<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AvailableVehicles { get; set; } = new List<SelectListItem>();
    }
}
