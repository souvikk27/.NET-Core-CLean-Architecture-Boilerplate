using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.OpenAPI.Auth.AuthEntity
{
    public class AuthCallback
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
        public string? AuthStatus { get; set; }
    }
}