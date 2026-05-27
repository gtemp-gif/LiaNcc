using Microsoft.Extensions.Configuration;

namespace LiaNcc.BO.Helpers
{
    public static class MediaUrlHelper
    {
        private static string? _fileBaseUrl;

        public static void Initialize(IConfiguration configuration)
        {
            // Use FileStorage:PublicBaseUrl if available, else fallback to PublicSiteBaseUrl + /LiaNccFiles
            _fileBaseUrl = configuration["FileStorage:PublicBaseUrl"]?.TrimEnd('/');

            if (string.IsNullOrEmpty(_fileBaseUrl))
            {
                var siteUrl = configuration["ApiSettings:PublicBaseUrl"]?.TrimEnd('/');
                _fileBaseUrl = $"{siteUrl}/LiaNccFiles";
            }
        }

        public static string GetAbsoluteUrl(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return string.Empty;
            if (relativePath.StartsWith("http", System.StringComparison.OrdinalIgnoreCase)) return relativePath;

            // Standardize path: should be relative to LiaNccFiles or start with /LiaNccFiles
            if (relativePath.StartsWith("/LiaNccFiles/"))
            {
                // If it already contains the base, we just need the domain part
                var domain = _fileBaseUrl?.Replace("/LiaNccFiles", "");
                return $"{domain}{relativePath}";
            }

            if (relativePath.StartsWith("LiaNccFiles/"))
            {
                var domain = _fileBaseUrl?.Replace("/LiaNccFiles", "");
                return $"{domain}/{relativePath}";
            }

            var cleanPath = relativePath.StartsWith("/") ? relativePath : "/" + relativePath;
            return $"{_fileBaseUrl}{cleanPath}";
        }
    }
}
