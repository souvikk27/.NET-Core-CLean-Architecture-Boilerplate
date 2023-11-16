using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public const string message = "Unauthorized access to resource";

        public UnauthorizedException() : base(message)
        { 
        }

        public UnauthorizedException(string resourceName) : base($"{message} {resourceName}") 
        { 
        }

        public UnauthorizedException(string resourceName, Exception innerException) : base($"{message} {resourceName}", innerException)
        {
        }
    }
}
