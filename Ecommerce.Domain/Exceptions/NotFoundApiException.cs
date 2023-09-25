using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Exceptions
{
    public class NotFoundApiException : Exception
    {
        private const string NotFoundMessage = "The requested resource was not found.";

        public NotFoundApiException() : base(NotFoundMessage)
        {
        }

        public NotFoundApiException(string resourceName) : base($"{NotFoundMessage} {resourceName}")
        {
        }

        public NotFoundApiException(string resourceName, Exception innerException) : base($"{NotFoundMessage} {resourceName}", innerException)
        {
        }
    }
}