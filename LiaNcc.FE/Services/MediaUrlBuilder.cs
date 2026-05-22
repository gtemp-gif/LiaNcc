namespace LiaNcc.FE.Services
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
            var url = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/";
            _baseUrl = url.TrimEnd('/');
        }

        public string? Build(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            return $"{_baseUrl}/{url.TrimStart('/')}";
        }
    }
}
