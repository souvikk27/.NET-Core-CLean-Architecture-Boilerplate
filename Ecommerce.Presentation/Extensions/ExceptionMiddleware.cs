using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.LoggerService;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Ecommerce.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Ecommerce.Domain.Errors;

namespace Ecommerce.Presentation.Extensions
{
    public static class ExceptionMiddleware
    {
        public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager logger)
        {
            app.UseExceptionHandler(appError => {
               appError.Run(async context => {
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if(contextFeature != null)
                {
                    context.Response.StatusCode = contextFeature.Error switch 
                    {
                        NotFoundApiException => StatusCodes.Status404NotFound,
                        BadRequestApiException => StatusCodes.Status400BadRequest,
                        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                        MutedApiException => StatusCodes.Status500InternalServerError,
                        _ => StatusCodes.Status500InternalServerError
                    };
                    logger.LogError($"Something went wrong {contextFeature.Error}");
                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        SattusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message,
                    }.ToString());
                }
               });
            });
        }
    }
}