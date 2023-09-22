using Ecommerce.Domain.Entities.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service.Extensions
{
    public static class ApiResponseExtension
    {
        private const string GenericSuccessMessage = "The operation completed succesfully.";
        private const string GenericWarningMessage = "The operation completed with warnings.";

        public static ObjectResult ToSuccessApiResult(object data, string message = null) =>
            new OkObjectResult(
                new ApiResponseModel<object>(
                    ApiResponseStatusEnum.Success,
                    message ?? GenericSuccessMessage,
                    data));

        public static ObjectResult ToWarningApiResult(object data, string message = null) =>
            new ObjectResult(
                new ApiResponseModel<object>(
                    ApiResponseStatusEnum.Warning,
                    message ?? GenericWarningMessage,
                    data));


        public static ObjectResult ToInfoApiResult(object data, string message = null) =>
            new OkObjectResult(
                new ApiResponseModel<object>(
                    ApiResponseStatusEnum.Info,
                    message,
                    data));

        public static ObjectResult ToErrorApiResult(object data, string message = null) =>
            new OkObjectResult(
                new ApiResponseModel<object>(
                    ApiResponseStatusEnum.Error,
                    message,
                    data));
    }
}
