using Ecommerce.LoggerService;
using Ecommerce.OpenAPI.Auth;
using Microsoft.AspNetCore.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;
using IAuthenticationService = Ecommerce.OpenAPI.Auth.Abstraction.IAuthenticationService;

namespace Ecommerce.Presentation.Controller;

[ApiController]
[Route("api/v1/[controller]")]

public class OpenIdController: ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthenticationService _authService;

    public OpenIdController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAuthenticationService authService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authService = authService;
    }

    [HttpPost("~/connect/token"), IgnoreAntiforgeryToken, Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request.IsPasswordGrantType())
        {
            var response = await _authService.AuthenticatePasswordGrantAsync(HttpContext, request.Username, request.Password);
            if (!response.Success)
            {
                return ApiResponseExtension.ToErrorApiResult(response.Properties, "OAuth2 Server Error", "404");
                
            }
            return SignIn(response.Principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else if (request.IsRefreshTokenGrantType())
        {
            var response = await _authService.AuthenticateRefreshTokenGrantAsync(HttpContext);
            if (!response.Success)
            {
                return ApiResponseExtension.ToErrorApiResult(response.Properties, "OAuth2 Server Error", "404");
            }

            return SignIn(response.Principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        throw new NotImplementedException("The specified grant type is not implemented.");
    }
}