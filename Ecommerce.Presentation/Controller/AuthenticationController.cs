using OpenIddict.Client.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        [HttpGet("~/callback/login/{provider}"), HttpPost("~/callback/login/{provider}"), IgnoreAntiforgeryToken]
        public async Task<ActionResult> LogInCallback()
        {
            // Retrieve the authorization data validated by OpenIddict as part of the callback handling.
            var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

            if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true })
            {
                throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
            }

            // Build an identity based on the external claims and that will be used to create the authentication cookie.
            var identity = new ClaimsIdentity(authenticationType: "ExternalLogin");

            identity.SetClaim(ClaimTypes.Email, result.Principal.GetClaim(ClaimTypes.Email))
                    .SetClaim(ClaimTypes.Name, result.Principal.GetClaim(ClaimTypes.Name))
                    .SetClaim(ClaimTypes.NameIdentifier, result.Principal.GetClaim(ClaimTypes.NameIdentifier));

            // Preserve the registration identifier to be able to resolve it later.
            identity.SetClaim(Claims.Private.RegistrationId, result.Principal.GetClaim(Claims.Private.RegistrationId));

            // Build the authentication properties based on the properties that were added when the challenge was triggered.
            var properties = new AuthenticationProperties(result.Properties.Items)
            {
                RedirectUri = result.Properties.RedirectUri ?? "/"
            };

            properties.StoreTokens(result.Properties.GetTokens().Where(token => token switch
            {
                // Preserve the access and refresh tokens returned in the token response, if available.
                {
                    Name: OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken or
                          OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken
                } => true,

                // Ignore the other tokens.
                _ => false
            }));
            return SignIn(new ClaimsPrincipal(identity), properties);
        }



        [HttpGet("~/callback/logout/{provider}"), HttpPost("~/callback/logout/{provider}"), IgnoreAntiforgeryToken]
        public async Task<ActionResult> LogOutCallback()
        {
            // Retrieve the data stored by OpenIddict in the state token created when the logout was triggered.
            var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

            // In this sample, the local authentication cookie is always removed before the user agent is redirected
            // to the authorization server. Applications that prefer delaying the removal of the local cookie can
            // remove the corresponding code from the logout action and remove the authentication cookie in this action.

            return Redirect(result!.Properties!.RedirectUri);
        }
    }
}
