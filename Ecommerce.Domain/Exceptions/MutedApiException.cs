using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Exceptions
{
    public class MutedApiException : Exception
    {
        private const string MutedMessage = "An error was encountered. Please contact support.";

        public MutedApiException() : base(MutedMessage)
        {
        }

        public MutedApiException(Exception innerException) : base(MutedMessage, innerException)
        {
        }
    }
}