using Ecommerce.Domain.Entities.Generic;

namespace Ecommerce.OpenAPI.Auth.Abstraction;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticatePasswordGrantAsync(HttpContext context, string username, string password);
    Task<AuthenticationResult> AuthenticateRefreshTokenGrantAsync(HttpContext context);
    Task<AuthenticationResult> AuthenticateClientCredentialGrantAsync(HttpContext context);
}