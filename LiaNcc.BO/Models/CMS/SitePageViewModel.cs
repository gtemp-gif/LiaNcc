using System;
using System.Collections.Generic;
using LiaNcc.BO.Models.Common;
using LiaNcc.BO.Models.Localization;

namespace LiaNcc.BO.Models.CMS
{
    public class SitePageViewModel : ILocalizedViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public bool IsActive { get; set; }

        public List<LocalizationTabViewModel> Translations { get; set; } = new();
    }
}
