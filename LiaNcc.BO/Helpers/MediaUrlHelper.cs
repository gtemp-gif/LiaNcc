using Microsoft.Extensions.Configuration;

namespace LiaNcc.BO.Helpers
{
    public static class MediaUrlHelper
    {
        private static string? _fileBaseUrl;

        public static void Initialize(IConfiguration configuration)
        {
            _fileBaseUrl = configuration["ApiSettings:FileBaseUrl"]?.TrimEnd('/');
        }

        public static string GetAbsoluteUrl(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return string.Empty;
            if (relativePath.StartsWith("http", System.StringComparison.OrdinalIgnoreCase)) return relativePath;

            var cleanPath = relativePath.StartsWith("/") ? relativePath : "/" + relativePath;
            return $"{_fileBaseUrl}{cleanPath}";
        }
    }
}
