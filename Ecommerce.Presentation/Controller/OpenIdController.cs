using System.Collections.Immutable;
using Ecommerce.OpenAPI.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Ecommerce.Presentation.Controller;

[ApiController]
[Route("api/v1/[controller]")]

public class OpenIdController: ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public OpenIdController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("~/connect/token"), IgnoreAntiforgeryToken, Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request.IsPasswordGrantType())
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                var properties = OpenIDAuthService.GetAuthenticationProperties("The specified user doesn't exist.");
                return ApiResponseExtension.ToErrorApiResult(properties, "OAuth2 Server Error", "404");
            }
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                var properties = OpenIDAuthService.GetAuthenticationProperties("The username/password couple is invalid.");

                return ApiResponseExtension.ToErrorApiResult(properties, "OAuth2 Server Error", "404");
            }

            var identity = await OpenIDAuthService.CreateClaimsIdentity(request, user, _userManager);
            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        
        else if (request.IsRefreshTokenGrantType())
        {
            // Retrieve the claims principal stored in the refresh token.
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Retrieve the user profile corresponding to the refresh token.
            var user = await _userManager.FindByIdAsync(result.Principal.GetClaim(Claims.Subject));
            if (user == null)
            {
                var properties = OpenIDAuthService.GetAuthenticationProperties("The refresh token is no longer valid.");

                return ApiResponseExtension.ToErrorApiResult(properties, "OAuth2 Server Error", "404");
            }

            // Ensure the user is still allowed to sign in.
            if (!await _signInManager.CanSignInAsync(user))
            {
                var properties = OpenIDAuthService.GetAuthenticationProperties("The user is no longer allowed to sign in.");

                return ApiResponseExtension.ToErrorApiResult(properties, "OAuth2 Server Error", "404");
            }

            var identity = await OpenIDAuthService.CreateClaimsIdentity(request, user, _userManager);

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        throw new NotImplementedException("The specified grant type is not implemented.");
    }  
}