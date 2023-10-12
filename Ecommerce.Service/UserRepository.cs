using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Ecommerce.Service
{
    public class UserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private const int RandomStringLength = 32;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> CreateUser(ApplicationUser user, string password)
        {
            user.Client_Id = GenerateRandomString();
            user.Client_Secret = GenerateRandomString();
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {  
                return user;
            }
            else
            return new ApplicationUser();
        }




        

        public static string GenerateRandomString()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[RandomStringLength];
                rng.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

    }
}
