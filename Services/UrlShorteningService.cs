using dotnet_url_shortener.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_url_shortener.Services
{
    public class UrlShorteningService(ShardService shardService)
    {
        private readonly Random _random = new();

        // Function that generates a unique code.
        public async Task<string> GenerateUniqueCode()
        {
            var codeChars = new char[ShortLinkSettings.Length];
            int maxValue = ShortLinkSettings.Alphabet.Length;

            while (true)
            {
                for (var i = 0; i < ShortLinkSettings.Length; i++)
                {
                    var randomIndex = _random.Next(maxValue);

                    codeChars[i] = ShortLinkSettings.Alphabet[randomIndex];
                }

                var code = new string(codeChars);

                var allUrls = await shardService.GetAllUrls();

                if (! allUrls.Any(s => s.Code == code))
                {
                    return code;
                }
            }
        }

    }
}
