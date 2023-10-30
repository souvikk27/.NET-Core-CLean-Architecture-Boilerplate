using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Ecommerce.LoggerService;
using Microsoft.EntityFrameworkCore;
using static Ecommerce.Service.Contract.Generators.TokenGenerator;
using Ecommerce.Service.Contract.Generators;
using Microsoft.Extensions.Configuration;
#pragma warning disable SYSLIB0023

namespace Ecommerce.Service
{
    public class UserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILoggerManager logger;
        private const int RandomStringLength = 32;
        private readonly IConfiguration configuration;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILoggerManager logger, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task<ApplicationUser> CreateUser(ApplicationUser user, string password)
        {
            var clientIdLength = 10;
            var clientSecretLength = 15;
            user.Client_Id = "app-api-" + GenerateRandomString(clientIdLength);
            user.Client_Secret = "app-api-" + GenerateRandomString(clientSecretLength);
            user.Refresh_Token = GenerateRefreshToken();

            if (user.Client_Id.Length >= user.Client_Secret.Length)
            {
                // Adjust the length of client_id to be less than client_secret
                user.Client_Id = "app-api-" + GenerateRandomString(clientSecretLength - 2);
            }
            try
            {
                var existingUser = _userManager.FindByNameAsync(user.UserName);
                if ( existingUser.Result == null)
                {
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        return user;
                    }
                    else
                        return new ApplicationUser();
                }
                else
                    return null;
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<ApplicationUser> GetById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user;
        }

        public async Task<ApplicationUser> Delete(Guid id)
        {
            var user = await GetById(id);
            var result = await _userManager.DeleteAsync(user);

            if(user == null)
            {
                return new ApplicationUser();
            }

            if(!result.Succeeded)
            {
                throw new InvalidOperationException("Something went wrong please try again later");
            }

            return user;
        }

        public async Task<Token> GetTokenAsync(string clientId, string clientSecret, string refreshToken)
        {
            var generator = new TokenGenerator(clientId, clientSecret, refreshToken);
            var token = await generator.GenerateAccessTokenAsync(configuration);
            return token;
        }

        public bool IsValid(string clientId, string clientSecret, string refreshToken)
        {
            return false;
        }

        
        private static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string GenerateRefreshToken()
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[64];
                cryptoProvider.GetBytes(bytes);

                string secureRandomString = Convert.ToBase64String(bytes);

                // Output example: Secure random string: OfGER+tSZIOSz314OlHk1aM+N8oNXDRHqTn3c5EVknYO5b5s0kqq40lJzoGj99ZXCvoFhkNG8KwQQvBPaR0FtQ==
                return secureRandomString;
            }
        }

    }
}
