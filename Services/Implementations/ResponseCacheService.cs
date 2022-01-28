using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace IssueTracker.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache distributedCache;

        public ResponseCacheService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public async Task CacheResponseAsync(string cachekey, object response, TimeSpan timeToLive)
        {

            if (response == null) return;

            var serializedResponse = JsonSerializer.Serialize(response);

            await distributedCache.SetStringAsync(cachekey, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeToLive
            }); ;
        }

        public async Task<string> GetCachedResponseBack(string cacheKey)
        {
            var cachedResponse = await distributedCache.GetStringAsync(cacheKey);

            return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
        }
    }
}
