using System;
using System.Collections.Generic;
using LiaNcc.BO.Models.Common;
using LiaNcc.BO.Models.Localization;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Models.Services
{
    public class ServiceViewModel : ILocalizedViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBookable { get; set; }
        public bool IsActive { get; set; }
        public string? CoverImageUrl { get; set; }
        public IFormFile? CoverImageFile { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }

        public List<LocalizationTabViewModel> Translations { get; set; } = new();
    }
}
