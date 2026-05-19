namespace dotnet_url_shortener.Models
{
    public class ShortenedUrl
    {
        public int Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public string Code { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int Clicks { get; set; } = 0;
    }
}
