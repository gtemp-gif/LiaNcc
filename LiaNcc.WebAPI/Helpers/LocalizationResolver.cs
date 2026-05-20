using System.Collections.Generic;
using System.Linq;
using LiaNcc.Models.Entities;

namespace LiaNcc.WebAPI.Helpers
{
    public interface ILocalizationResolver
    {
        string Resolve(IEnumerable<LocalizedContent> translations, string contentKey, string fallback, string? culture);
    }

    public class LocalizationResolver : ILocalizationResolver
    {
        public string Resolve(IEnumerable<LocalizedContent> translations, string contentKey, string fallback, string? culture)
        {
            if (string.IsNullOrEmpty(culture)) return fallback;

            var translation = translations.FirstOrDefault(t =>
                t.LanguageCode.Equals(culture, System.StringComparison.OrdinalIgnoreCase) &&
                t.ContentKey.Equals(contentKey, System.StringComparison.OrdinalIgnoreCase));

            return translation?.ContentValue ?? fallback;
        }
    }
}
