using System.Collections.Generic;

namespace LiaNcc.BO.Models.Localization
{
    public class LocalizationTabViewModel
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public Dictionary<string, string?> Values { get; set; } = new();
    }
}
