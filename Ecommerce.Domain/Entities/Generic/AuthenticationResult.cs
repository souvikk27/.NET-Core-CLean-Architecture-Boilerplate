using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
namespace Ecommerce.Domain.Entities.Generic;

public class AuthenticationResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public ClaimsPrincipal Principal { get; set; }
    
    public AuthenticationProperties? Properties { get; set; }

    public AuthenticationResult()
    {
    }
    
    /// <summary>
    /// Represents the result of an authentication operation.
    /// </summary>
    public AuthenticationResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }
    
    /// <summary>
    /// Represents the result of an authentication operation with claims principal.
    /// </summary>
    public AuthenticationResult(bool success, string message, ClaimsPrincipal principal)
    {
        Success = success;
        Message = message;
        Principal = principal;
    }
    
    /// <summary>
    /// Represents the result of an authentication operation with authentication properties.
    /// </summary>
    public AuthenticationResult(bool success, string message,  AuthenticationProperties properties)
    {
        Success = success;
        Message = message;
        Properties = properties;
    }
}