using System;
using System.Collections.Generic;
using LiaNcc.BO.Models.Common;
using LiaNcc.BO.Models.Localization;

namespace LiaNcc.BO.Models.Partners
{
    public class PartnerViewModel : ILocalizedViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? WebsiteUrl { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }

        public List<LocalizationTabViewModel> Translations { get; set; } = new();
    }
}
