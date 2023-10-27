using Ecommerce.Domain.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;

public class RateLimitAttribute : Attribute, IActionFilter
{
    private readonly int _limit;
    private readonly TimeSpan _period;
    private readonly string _cacheKey;

    public RateLimitAttribute(int limit, int periodInSeconds)
    {
        _limit = limit;
        _period = TimeSpan.FromSeconds(periodInSeconds);
        _cacheKey = $"RateLimit_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}";
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var cache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

        if (cache.TryGetValue(_cacheKey, out int currentCount))
        {
            ErrorDetails details = new ErrorDetails()
            {
                SattusCode = 429,
                Message = "Request quota has been exceeded."
            };
            if (currentCount >= _limit)
            {
                context.Result = new ContentResult
                {
                    Content = details.ToString(),
                    StatusCode = 429, 
                    ContentType = "application/json"
                };
                return;
            }
        }
        else
        {
            currentCount = 0;
        }

        cache.Set(_cacheKey, currentCount + 1, _period);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // You can add post-execution logic here if needed
    }
}
