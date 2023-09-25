using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ecommerce.LoggerService
{
    public class LoggerManager : ILoggerManager
    {
        private readonly ILogger<LoggerManager> logger;

        public LoggerManager(ILogger<LoggerManager> logger)
        {
            this.logger = logger;
        }

        public void LogDebug(string message)
        {
            logger.LogDebug(message);
        }

        public void LogError(string message)
        {
            logger.LogError(message);
        }

        public void LogInformation(string message)
        {
            logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            logger.LogWarning(message);
        }
    }
}