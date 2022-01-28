using IssueTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace IssueTracker.Cache
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        public int timeToLiveSeconds { get; set; }
        // Cant use ctor DI, would need to injected whereever it's used.  TLDR: Not Possible.

        public CachedAttribute(int timeToLiveSeconds)
        {
            this.timeToLiveSeconds = timeToLiveSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
			var cacheSettings = context.HttpContext.RequestServices.GetRequiredService<RedisCacheSettings>();

			if (!cacheSettings.isEnabled)
			{
				await next();
				return;
			}

			var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

			var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
			var cachedResponse = await cacheService.GetCachedResponseBack(cacheKey);

			if (!string.IsNullOrEmpty(cachedResponse))
			{
				var contentResult = new ContentResult
				{
					Content = cachedResponse,
					ContentType = "application/json",
					StatusCode = 200
				};
				context.Result = contentResult;
				return;
			}

			var executedContext = await next();

			if (executedContext.Result is OkObjectResult okObjectResult)
			{
				await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
			}
		}

		private static string GenerateCacheKeyFromRequest(HttpRequest request)
		{
			var keyBuilder = new StringBuilder();

			keyBuilder.Append($"{request.Path}");

			foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
			{
				keyBuilder.Append($"|{key}-{value}");
			}

			return keyBuilder.ToString();
		}
	}
}
