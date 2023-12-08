using static OpenIddict.Abstractions.OpenIddictConstants;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;
using ClaimsPrincipal = System.Security.Claims.ClaimsPrincipal;
using Ecommerce.OpenAPI.Auth.Abstraction;
using ClaimsIdentity = System.Security.Claims.ClaimsIdentity;

namespace Ecommerce.OpenAPI.Auth.Services
{
    public class OpenIdValidationService : IOpenIdValidationService
    {
        private readonly ILoggerManager _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public OpenIdValidationService(ILoggerManager logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

    }
}
