using Microsoft.Extensions.Configuration;

namespace LiaNcc.FE.Helpers
{
    public interface IMediaUrlBuilder
    {
        string? Build(string? url);
    }

    public class MediaUrlBuilder : IMediaUrlBuilder
    {
        private readonly string _fileBaseUrl;

        public MediaUrlBuilder(IConfiguration configuration)
        {
            // Use FileStorage:PublicBaseUrl if available, else fallback to PublicSiteBaseUrl + /LiaNccFiles
            var fileUrl = configuration["FileStorage:PublicBaseUrl"]?.TrimEnd('/');

            if (string.IsNullOrEmpty(fileUrl))
            {
                var siteUrl = configuration["ApiSettings:PublicBaseUrl"]?.TrimEnd('/');
                fileUrl = $"{siteUrl}/LiaNccFiles";
            }
            _fileBaseUrl = fileUrl;
        }

        public string? Build(string? url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (url.StartsWith("http://") || url.StartsWith("https://")) return url;

            // Standardize path: should be relative to LiaNccFiles or start with /LiaNccFiles
            if (url.StartsWith("/LiaNccFiles/"))
            {
                // If it already contains the base, we just need the domain part
                var domain = _fileBaseUrl.Replace("/LiaNccFiles", "");
                return $"{domain}{url}";
            }

            if (url.StartsWith("LiaNccFiles/"))
            {
                var domain = _fileBaseUrl.Replace("/LiaNccFiles", "");
                return $"{domain}/{url}";
            }

            var path = url.StartsWith("/") ? url : "/" + url;
            return _fileBaseUrl + path;
        }
    }
}
