using dotnet_url_shortener.Data;
using dotnet_url_shortener.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace dotnet_url_shortener.Services
{
    public class ShardService
    {
        private readonly ShardAContext _contextA;
        private readonly ShardBContext _contextB;

        public ShardService(ShardAContext contextA, ShardBContext contextB)
        {
            _contextA = contextA;
            _contextB = contextB;
        }

        // Selector method to determine which shard an item is in.
        public DbContext GetShardContext(int shardId)
        {
            return shardId % 2 == 0 ? _contextA : _contextB;
        }

        // Get list of all Urls to check for concurrency issues.
        public async Task<List<ShortenedUrl>> GetAllUrls()
        {
            var shardA = await _contextA.ShortenedUrls.ToListAsync();
            var shardB = await _contextB.ShortenedUrls.ToListAsync();

            return shardA.Concat(shardB).ToList();
        }

        // Get the next free Id in the database.
        public async Task<int> GetNextFreeId()
        {
            int countA = await _contextA.ShortenedUrls.CountAsync();
            int countB = await _contextB.ShortenedUrls.CountAsync();

            return countA + countB + 1;
        }
    }
}
