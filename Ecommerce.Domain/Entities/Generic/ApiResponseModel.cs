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

        public T Data { get; set; }

        public ApiResponseModel(ApiResponseStatusEnum status, string message, T data, Guid? apiResponseId = null)
        {
            ApiResponseId = apiResponseId ?? Guid.NewGuid();
            Status = Enum.GetName(typeof(ApiResponseStatusEnum), status)?.ToLower();
            StatusCode = (int)status;
            Message = message;
            Data = data;
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
