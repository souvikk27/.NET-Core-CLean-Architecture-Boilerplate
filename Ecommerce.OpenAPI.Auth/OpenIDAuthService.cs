using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Collections.Immutable;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Ecommerce.OpenAPI.Auth;

public static class OpenIDAuthService
{
    public static AuthenticationProperties GetAuthenticationProperties(string message)
    {
        var properties = new AuthenticationProperties(new Dictionary<string, string>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
            message 
        }!);
        return properties;
    }

    public static async Task<ClaimsIdentity> CreateClaimsIdentity(OpenIddictRequest request,ApplicationUser user, UserManager<ApplicationUser> _userManager)
    {
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
        nameType: Claims.Name,
            roleType: Claims.Role);

        // Add the claims that will be persisted in the tokens.
        identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
        .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
            .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
            .SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

        identity.SetScopes(request.GetScopes());
        identity.SetDestinations(OpenIDAuthService.GetDestinations);

        return identity;
    }

    public static IEnumerable<string> GetDestinations(Claim claim)
    {

        switch (claim.Type)
        {
            case Claims.Name:
                yield return Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Profile))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Email))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Roles))
                    yield return Destinations.IdentityToken;

                yield break;

            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}