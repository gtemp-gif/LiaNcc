using System;
using System.Collections.Generic;
using LiaNcc.BO.Models.Common;
using LiaNcc.BO.Models.Localization;
using LiaNcc.Models.DTOs.Vehicles;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Models.Vehicles
{
    public class VehicleViewModel : ILocalizedViewModel
    {
        public Guid? Id { get; set; }
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

        public List<VehicleCategory> AvailableCategories { get; set; } = new();
        public List<VehicleFeatureDto> Features { get; set; } = new();
        public List<VehicleGalleryImageDto> ExistingGalleryImages { get; set; } = new();

        public List<IFormFile> NewGalleryImages { get; set; } = new();

        // For adding new feature inline
        public string? NewFeatureName { get; set; }
        public string? NewFeatureIcon { get; set; }

        public List<LocalizationTabViewModel> Translations { get; set; } = new();
    }
}
