using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTO.Users;

public class UserLoginDto
{
    [Required(ErrorMessage = "grant_type is required")]
    public string GrantType { get; set; }
    [Required(ErrorMessage = "username is required")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "password is required")]
    public string Password { get; set; }
    public string? Scope { get; set; }
}