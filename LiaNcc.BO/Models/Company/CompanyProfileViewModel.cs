using System;
using System.Collections.Generic;
using LiaNcc.BO.Models.Common;
using LiaNcc.BO.Models.Localization;

namespace LiaNcc.BO.Models.Company
{
    public class CompanyProfileViewModel : ILocalizedViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? VatNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }

        public List<LocalizationTabViewModel> Translations { get; set; } = new();
    }
}
