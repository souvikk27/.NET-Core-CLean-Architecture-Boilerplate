using Ecommerce.Domain.Entities;
using Ecommerce.LoggerService;
using Ecommerce.Presentation.Infrastructure.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Presentation.Infrastructure.Services
{
    public class ClientCredentialService : IClientCredentialService
    {
        private readonly ILoggerManager logger;

        public ClientCredentialService(ILoggerManager logger)
        {
            this.logger = logger;
        }


        public async Task<OAuthClient> InvokeCredentialsAsync()
        {
            var client = new OAuthClient();
            const int clientIdLength = 25;
            const int clientSecretLength = 40;
            client.Id = Guid.NewGuid();
            client.Clientid = "app-api-" + GenerateRandomString(clientIdLength);
            client.Clientsecret = "app-api-" + GenerateRandomString(clientSecretLength);
            client.AddedOn = DateTime.Now;
            client.isActive = true;

            return await Task.FromResult(client);
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
