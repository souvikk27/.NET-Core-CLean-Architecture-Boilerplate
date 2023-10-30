using Ecommerce.Domain.Entities.Generic;
using Ecommerce.Domain.Errors;
using Ecommerce.Service.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
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
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        var apiOperation = actionDescriptor?.ActionName;
        var apiEndpoint = context.HttpContext.Request.Path;
        var cache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

        if (cache.TryGetValue(_cacheKey, out int currentCount))
        {
            ResponseModel model = new ResponseModel
            {
                ApiOperation = apiOperation,
                ApiEndpoint = apiEndpoint.ToString()
            };
            if (currentCount >= _limit)
            {
                context.Result = ApiResponseExtension.ToErrorApiResult(model, "Request Quota Exceeded", "429");
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

    private class ResponseModel
    {
        public string ApiOperation { get; set; }
        public string ApiEndpoint { get; set; }
        
    }

}
