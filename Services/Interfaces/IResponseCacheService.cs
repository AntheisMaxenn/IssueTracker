namespace IssueTracker.Services
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cachekey, object response, TimeSpan timeToLive);

        Task<string> GetCachedResponseBack(string cacheKey);
    }
}
