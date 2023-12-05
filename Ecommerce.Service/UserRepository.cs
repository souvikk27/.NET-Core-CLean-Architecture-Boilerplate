using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Ecommerce.LoggerService;
using Microsoft.EntityFrameworkCore;
using static Ecommerce.Service.Contract.Generators.TokenGenerator;
using Ecommerce.Service.Contract.Generators;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using Microsoft.Extensions.Options;
using OpenIddict.Server;
using Microsoft.IdentityModel.Tokens;

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
        IOptionsMonitor<OpenIddictServerOptions> _oidcOptions;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
            ILoggerManager logger, IConfiguration configuration, IOptionsMonitor<OpenIddictServerOptions> oidcOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.logger = logger;
            this.configuration = configuration;
            _oidcOptions = oidcOptions;
        }

        public async Task<ApplicationUser> CreateUser(ApplicationUser user, string password)
        {
            try
            {
                var existingUser = await _userManager.FindByNameAsync(user.UserName);
                if ( existingUser == null)
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


        public async Task<Token> GetTokenAsync(string clientId, string clientSecret)
        {
            var generator = new TokenGenerator(clientId, clientSecret);
            var token = await generator.GenerateAccessTokenAsync(configuration);
            return token;
        }


        public bool IsValid(string clientId, string clientSecret, string refreshToken)
        {
            return false;
        }
    }
}
