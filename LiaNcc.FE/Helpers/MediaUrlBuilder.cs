using Microsoft.Extensions.Configuration;

namespace LiaNcc.FE.Helpers
{
    public interface IMediaUrlBuilder
    {
        string? Build(string? url);
    }

    public class MediaUrlBuilder : IMediaUrlBuilder
    {
        private readonly string _baseUrl;

        public MediaUrlBuilder(IConfiguration configuration)
        {
            var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/api/";
            // BaseUrl for media should be the root of the API, not /api/
            _baseUrl = apiBaseUrl.Replace("/api/", "/").TrimEnd('/');
        }

        public string? Build(string? url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (url.StartsWith("http://") || url.StartsWith("https://")) return url;

            var path = url.StartsWith("/") ? url : "/" + url;
            return _baseUrl + path;
        }
    }
}
