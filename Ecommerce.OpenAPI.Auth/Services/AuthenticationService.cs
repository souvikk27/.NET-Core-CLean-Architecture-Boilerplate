using System.Security.Claims;
using Ecommerce.Domain.Entities.Generic;
using Microsoft.AspNetCore;
using IAuthenticationService = Ecommerce.OpenAPI.Auth.Abstraction.IAuthenticationService;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Immutable;
using Azure.Core;
using Ecommerce.Service.Context;
namespace Ecommerce.OpenAPI.Auth.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILoggerManager _logger;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly ApplicationContext _context;

    public AuthenticationService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        ILoggerManager logger, IOpenIddictApplicationManager applicationManager, ApplicationContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _applicationManager = applicationManager;
        _context = context;
    }


    public async Task<AuthenticationResult> AuthenticatePasswordGrantAsync(HttpContext context, string username,
        string password)
    {
        var request = context.GetOpenIddictServerRequest();
        var user = await _userManager.FindByNameAsync(request?.Username);
        if (user == null)
        {
            const string message = "The specified user doesn't exist.";
            _logger.LogError(message);
            var properties = OpenIDAuthService.GetAuthenticationProperties(message);
            return new AuthenticationResult(false, message, properties);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            const string message = "The username/password couple is invalid.";
            _logger.LogError(message);
            var properties =
                OpenIDAuthService.GetAuthenticationProperties(message);
            return new AuthenticationResult(false, message, properties);
        }
        var identity = await OpenIDAuthService.CreateClaimsIdentity(request, user, _userManager);
        var principal = new ClaimsPrincipal(identity);
        return new AuthenticationResult( true, "Authentication Succeeded", principal);
    }


    public async Task<AuthenticationResult> AuthenticateRefreshTokenGrantAsync(HttpContext context)
    {
        var request = context.GetOpenIddictServerRequest();

        if(request?.ClientId != null)
        {
            var clientIdentity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            clientIdentity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId);
            var clientPrincipal = new ClaimsPrincipal(clientIdentity);
            clientPrincipal.SetScopes(request.GetScopes());
            return new AuthenticationResult(true, "Authentication Succeeded", clientPrincipal);
        }

        var result = await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var user = await _userManager.FindByIdAsync(result.Principal.GetClaim(Claims.Subject));
        if (user == null)
        {
            const string message = "The specified user doesn't exist.";
            var properties = OpenIDAuthService.GetAuthenticationProperties(message);
            _logger.LogError(message);
            return new AuthenticationResult(false, message, properties);
        }

        // Ensure the user is still allowed to sign in.
        if (!await _signInManager.CanSignInAsync(user))
        {
            const string message = "The user is no longer allowed to sign in.";
            var properties = OpenIDAuthService.GetAuthenticationProperties(message);
            _logger.LogError(message);
            return new AuthenticationResult(false, message, properties);
        }

        var identity = await OpenIDAuthService.CreateClaimsIdentity(request, user, _userManager);
        var principal = new ClaimsPrincipal(identity);
        return new AuthenticationResult(true, "Authentication Succeeded", principal);
    }


    public async Task<AuthenticationResult> AuthenticateClientCredentialGrantAsync(HttpContext context)
    {
        var request = context.GetOpenIddictServerRequest();
        var client = await _applicationManager.FindByClientIdAsync(request?.ClientId);
        if (client == null)
        {
            const string message = "The specified client doesn't exist.";
            var properties = OpenIDAuthService.GetAuthenticationProperties(message);
            _logger.LogError(message);
            return new AuthenticationResult(false, message, properties);
        }

        var userId = _context.OAuthClient
            .Where(x => x.Clientid == request.ClientId)
            .Select(x => x.UserId)
            .FirstOrDefault();
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            const string message = "The specified user doesn't exist.";
            var properties = OpenIDAuthService.GetAuthenticationProperties(message);
            _logger.LogError(message);
            return new AuthenticationResult(false, message, properties);
        }

        var identity = await OpenIDAuthService.CreateClaimsIdentity(request, user, _userManager);
        identity.AddClaim(OpenIddictConstants.Claims.Subject, user.Id);
        var principal = new ClaimsPrincipal(identity);

        principal.SetScopes(request.GetScopes());
        return new AuthenticationResult(true, "Authentication Succeeded", new ClaimsPrincipal(identity));
    }


    public async Task<AuthenticationResult> AuthenticateCodeGrantAsync(HttpContext context)
    {
        var request = context.GetOpenIddictServerRequest();
        // Retrieve the claims principal stored in the authorization code
        var result = await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        // Retrieve the user profile corresponding to the authorization code/refresh token.
        var user = await _userManager.FindByIdAsync(result.Principal.GetClaim(Claims.Subject));
        if (user is null)
        {
            const string message = "The specified user doesn't exist.";
            var properties = OpenIDAuthService.GetAuthenticationProperties(message);
            _logger.LogError(message);
            return new AuthenticationResult(false, message, properties);
        }

        // Ensure the user is still allowed to sign in.
        if (!await _signInManager.CanSignInAsync(user))
        {
            const string message = "The user is no longer allowed to sign in.";
            var properties = OpenIDAuthService.GetAuthenticationProperties(message);
            _logger.LogError(message);
            return new AuthenticationResult(false, message, properties);
        }

        var identity = new ClaimsIdentity(result.Principal.Claims,
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Override the user claims present in the principal in case they
        // changed since the authorization code/refresh token was issued.
        identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                .SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

        identity.SetDestinations(OpenIDAuthService.GetDestinations);

        var principal = new ClaimsPrincipal(identity);

        principal.SetScopes(request.GetScopes());

        return new AuthenticationResult(true, "Authentication Succeeded", new ClaimsPrincipal(identity));
    }


    private bool ValidatePkce(string codeChallenge, string codeChallengeMethod, string codeVerifier)
    {
        // Assuming codeVerifier is stored somewhere (e.g., in a database or a secure cache)
        string storedCodeVerifier = codeVerifier ?? string.Empty;

        // If the code challenge method is plain, the code challenge is the same as the code verifier.
        if (codeChallengeMethod == OpenIddictConstants.CodeChallengeMethods.Plain)
        {
            return string.Equals(codeChallenge, storedCodeVerifier, StringComparison.Ordinal);
        }
        // If the code challenge method is S256, SHA256-hash the code verifier and compare it with the code challenge.
        else if (codeChallengeMethod == OpenIddictConstants.CodeChallengeMethods.Sha256)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(storedCodeVerifier));
                string hashedCodeVerifier = Base64UrlEncoder.Encode(hashedBytes);

                return string.Equals(codeChallenge, hashedCodeVerifier, StringComparison.Ordinal);
            }
        }
        // Unsupported code challenge method
        return false;
    }

}
