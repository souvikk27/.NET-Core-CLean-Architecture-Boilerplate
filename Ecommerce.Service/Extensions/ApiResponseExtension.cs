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

        public static ObjectResult ToSuccessApiResult(object data, string message = null, string statuscode = null) =>
            new OkObjectResult(
                new ApiResponseModel<object>(
                    ApiResponseStatusEnum.Success,
                    message ?? GenericSuccessMessage,
                    data,
                    statuscode));

        public static ObjectResult ToWarningApiResult(object data, string message = null, string statuscode = null) =>
            new ObjectResult(
                new ApiResponseModel<object>(
                    ApiResponseStatusEnum.Warning,
                    message ?? GenericWarningMessage,
                    data,
                    statuscode));


        public static ObjectResult ToInfoApiResult(object data, string message = null, string statuscode = null) =>
            new OkObjectResult(
                new ApiResponseModel<object>(
                    ApiResponseStatusEnum.Info,
                    message,
                    data,
                    statuscode));

        public static ObjectResult ToErrorApiResult(object data, string message = null, string statuscode = null) =>
            new OkObjectResult(
                new ApiResponseModel<object>(
                    ApiResponseStatusEnum.Error,
                    message,
                    data,
                    statuscode));
    }
}
