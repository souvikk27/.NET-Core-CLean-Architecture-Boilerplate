using Ecommerce.OpenAPI.Auth;
using Ecommerce.OpenAPI.Auth.AuthEntity;
using Ecommerce.Presentation.Extensions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using System.Collections.Immutable;
using System.Net;
using static OpenIddict.Abstractions.OpenIddictConstants;
using IAuthenticationService = Ecommerce.OpenAPI.Auth.Abstraction.IAuthenticationService;



namespace Ecommerce.Presentation.Controller;

[ApiController]
[Route("api/v1/oAuth2")]

public class OpenIdController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    IAuthenticationService _authService;

    public OpenIdController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAuthenticationService authService, IOpenIddictApplicationManager applicationManager,
    IOpenIddictAuthorizationManager authorizationManager, IOpenIddictScopeManager scopeManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authService = authService;
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
    }

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
        throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (!result.Succeeded)
        {

            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri =  Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                });
        }

        var user = await _userManager.GetUserAsync(result.Principal) ??
            throw new InvalidOperationException("The user details cannot be retrieved.");

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
            throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

        var authorizations = await _authorizationManager.FindAsync(
            subject: await _userManager.GetUserIdAsync(user),
            client: await _applicationManager.GetIdAsync(application),
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: request.GetScopes()).ToListAsync();

        switch (await _applicationManager.GetConsentTypeAsync(application))
        {
            case ConsentTypes.External when !authorizations.Any():
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application."
                    }));


            case ConsentTypes.Implicit:
            case ConsentTypes.External when authorizations.Any():
            case ConsentTypes.Explicit when authorizations.Any() && !request.HasPrompt(Prompts.Consent):
                // Create the claims-based identity that will be used by OpenIddict to generate tokens.
                var identity = new ClaimsIdentity(
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);

                // Add the claims that will be persisted in the tokens.
                identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                        .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                        .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                        .SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

                // Note: in this sample, the granted scopes match the requested scope
                // but you may want to allow the user to uncheck specific scopes.
                // For that, simply restrict the list of scopes before calling SetScopes.
                identity.SetScopes(request.GetScopes());
                identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

                var auth = authorizations.LastOrDefault();
                auth ??= _authorizationManager.CreateAsync(
                    identity: identity,
                    subject: await _userManager.GetUserIdAsync(user),
                    client: await _applicationManager.GetIdAsync(application),
                    type: AuthorizationTypes.Permanent,
                    scopes: identity.GetScopes());

                identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(auth));
                identity.SetDestinations(OpenIDAuthService.GetDestinations);

                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            case ConsentTypes.Explicit when request.HasPrompt(Prompts.None):
            case ConsentTypes.Systematic when request.HasPrompt(Prompts.None):
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Interactive user consent is required."
                    }));

            // In every other case, render the consent form.
            default:
                return Ok(new AuthorizeDto
                {
                    ApplicationName = await _applicationManager.GetLocalizedDisplayNameAsync(application),
                    Scope = request.Scope
                });
        }
    }


    [Authorize, FormValueRequired("submit.Accept")]
    [HttpPost("~/connect/authorize"), ValidateAntiForgeryToken]

    public async Task<IActionResult> Accept()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the profile of the logged in user.
        var user = await _userManager.GetUserAsync(User) ??
            throw new InvalidOperationException("The user details cannot be retrieved.");

        // Retrieve the application details from the database.
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
            throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

        // Retrieve the permanent authorizations associated with the user and the calling client application.
        var authorizations = await _authorizationManager.FindAsync(
            subject: await _userManager.GetUserIdAsync(user),
            client: await _applicationManager.GetIdAsync(application),
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: request.GetScopes()).ToListAsync();

        // Note: the same check is already made in the other action but is repeated
        // here to ensure a malicious user can't abuse this POST-only endpoint and
        // force it to return a valid response without the external authorization.
        if (!authorizations.Any() && await _applicationManager.HasConsentTypeAsync(application, ConsentTypes.External))
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The logged in user is not allowed to access this client application."
                }));
        }

        // Create the claims-based identity that will be used by OpenIddict to generate tokens.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Add the claims that will be persisted in the tokens.
        identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                .SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

        // Note: in this sample, the granted scopes match the requested scope
        // but you may want to allow the user to uncheck specific scopes.
        // For that, simply restrict the list of scopes before calling SetScopes.
        identity.SetScopes(request.GetScopes());
        identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

        // Automatically create a permanent authorization to avoid requiring explicit consent
        // for future authorization or token requests containing the same scopes.
        var authorization = authorizations.LastOrDefault();
        authorization ??= await _authorizationManager.CreateAsync(
            identity: identity,
            subject: await _userManager.GetUserIdAsync(user),
            client: await _applicationManager.GetIdAsync(application),
            type: AuthorizationTypes.Permanent,
            scopes: identity.GetScopes());

        identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));
        identity.SetDestinations(OpenIDAuthService.GetDestinations);

        // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [Authorize, FormValueRequired("submit.Deny")]
    [HttpPost("~/connect/authorize"), ValidateAntiForgeryToken]
    // Notify OpenIddict that the authorization grant has been denied by the resource owner
    // to redirect the user agent to the client application using the appropriate response_mode.
    public IActionResult Deny() => Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);



    [HttpPost("~/connect/logout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutPost()
    {
        await _signInManager.SignOutAsync();
        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    }



    [HttpPost("~/connect/token"), IgnoreAntiforgeryToken, Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request.IsClientCredentialsGrantType())
        {
            var response = await _authService.AuthenticateClientCredentialGrantAsync(HttpContext);
            if (!response.Success)
            {
                return ApiResponseExtension.ToErrorApiResult(response.Properties, "OAuth2 Server Error", "404");
            }
            return SignIn(response.Principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsAuthorizationCodeGrantType())
        {


        }


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

    [HttpGet("~/signin-oidc")]
    public async Task<IActionResult> Callback(string redirectUrl = null)
    {

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            var url = new CallbackLinkDto
            {
                ReturnURL = Url.Action("Login"),
                Rel = "login",
                Method = "POST"
            };
            return ApiResponseExtension.ToInfoApiResult(url);
        }

        return ApiResponseExtension.ToErrorApiResult("OpenId Connect", "Url Redirection Failed", "404");
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route("~/api/oauth/v2/login")]
    public async Task<IActionResult> Login([FromForm] AuthCallback authCallback, string returnUrl = null)
    {
        authCallback.ReturnUrl = returnUrl;
        if (ModelState.IsValid)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(authCallback.UserName, authCallback.Password, authCallback.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                CallbackLinkDto callbackLinkDto = new CallbackLinkDto
                {
                    ReturnURL = returnUrl,
                    Rel = "authorize",
                    Method = "GET"
                };
                return Ok(callbackLinkDto);
            }
            //if (result.RequiresTwoFactor)
            //{
            //    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = authCallback.RememberMe });
            //}
            if (result.IsLockedOut)
            {
                return ApiResponseExtension.ToErrorApiResult("OAuth Server Violation", "User is locked out", "401");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return ApiResponseExtension.ToErrorApiResult("OAuth Server Violation", "Invalid Login Attempt", "401");
            }
        }

        // If we got this far, something failed, redisplay form
        return ApiResponseExtension.ToErrorApiResult("OAuth Server Violation", "Internal Server Error. Please try again after sometime", "500");
    }
}