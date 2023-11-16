using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities.Generic
{
    public class ApiResponseModel<T>
    {
        public Guid ApiResponseId { get; set; }

        public string Status { get; set; }

        public int StatusCode { get; set; }

        public string Message { get; set; }

        public T Payload { get; set; }

        public ApiResponseModel(ApiResponseStatusEnum status, string message, T payload, string? statusCode = null, Guid? apiResponseId = null)
        {
            ApiResponseId = apiResponseId ?? Guid.NewGuid();
            Status = Enum.GetName(typeof(ApiResponseStatusEnum), status)?.ToLower();
            StatusCode = statusCode != null ? Convert.ToInt32(statusCode) : GetStatusCode(status);
            Message = message;
            Payload = payload;
        }

        public int GetStatusCode(ApiResponseStatusEnum status)
        {
            int statusCode = 0;
            switch (status)
            {
                case ApiResponseStatusEnum.Success:
                    statusCode = 200;
                    break;
                case ApiResponseStatusEnum.Warning:
                    statusCode = 429;
                    break;
                case ApiResponseStatusEnum.Error:
                    statusCode = 400;
                    break;
                default:
                    statusCode = 100;
                    break;
            }
            return statusCode;
        }
    }

    public enum ApiResponseStatusEnum
    {
        NotSet = 0,
        Success,
        Warning,
        Info,
        Error
    }
}
