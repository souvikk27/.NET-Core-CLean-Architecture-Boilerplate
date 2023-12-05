using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Ecommerce.LoggerService;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Service.Context;


#pragma warning disable SYSLIB0023

namespace Ecommerce.Service
{
    public class UserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILoggerManager logger;
        private readonly ApplicationContext _context;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
            ILoggerManager logger, ApplicationContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            this.logger = logger;
        }

        public async Task<ApplicationUser> CreateUser(ApplicationUser user, string password, OAuthClient client)
        {
            try
            {
                var existingUser = await _userManager.FindByNameAsync(user.UserName);
                if ( existingUser == null)
                {
                    user.OAuthClient = client;
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await _context.OAuthClient.AddAsync(client);
                        await _context.SaveChangesAsync();
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

            if (user == null)
            {
                return new ApplicationUser();
            }

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Something went wrong please try again later");
            }

            return user;
        }

        public bool IsValid(string clientId, string clientSecret, string refreshToken)
        {
            return false;
        }
    }
}
