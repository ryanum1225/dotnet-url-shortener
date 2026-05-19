using dotnet_url_shortener.Data;
using dotnet_url_shortener.Models;
using dotnet_url_shortener.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace dotnet_url_shortener.Controllers
{
    [ApiController]
    [Route("url")]
    public class ShortenerController : ControllerBase
    {
        private ShardService _shard;
        private UrlShorteningService _url;
        private IMemoryCache _cache;

        public ShortenerController(ShardService shard, UrlShorteningService url, IMemoryCache Cache)
        {
            _shard = shard;
            _url = url;
            _cache = Cache;
        }

        // Method that takes a URL and shortens it.
        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenRequest request)
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri))
            {
                return BadRequest("Invalid URL format");
            }

            var id = await _shard.GetNextFreeId();
            var code = await _url.GenerateUniqueCode();

            var shortUrl = new ShortenedUrl
            {
                Id = id,
                LongUrl = request.Url,
                Code = code,
                ShortUrl = $"{Request.Scheme}://{Request.Host}/{code}",
                CreatedOnUtc = DateTime.UtcNow
            };

            var context = _shard.GetShardContext(id);
            context.Set<ShortenedUrl>().Add(shortUrl);
            await context.SaveChangesAsync();

            return Ok(shortUrl.ShortUrl);
        }

        // Endpoint to redirect the user to their long link when short link is inputted.
        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectUrl(string code)
        {
            if (!_cache.TryGetValue(code, out ShortenedUrl url))
            {
                var allUrls = await _shard.GetAllUrls();
                url = allUrls.FirstOrDefault(s => s.Code == code);

                if (url == null)
                {
                    return NotFound();
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(code, url, cacheEntryOptions);

            }

            url.Clicks++;

            return Redirect(url.LongUrl);
        }
    }
}

public record ShortenRequest(string Url);