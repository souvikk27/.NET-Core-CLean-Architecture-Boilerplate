using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.LoggerService
{
    public interface ILoggerManager
    {
        void LogInformation(string message);
        void LogDebug(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}