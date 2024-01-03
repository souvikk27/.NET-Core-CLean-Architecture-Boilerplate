using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.OpenAPI.Auth.AuthEntity
{
    public class AuthRequest
    {
        [Required(ErrorMessage = "GrantType is required.")]
        public string GrantType { get; set; }

        [Required(ErrorMessage = "ClientId is required.")]
        public string ClientId { get; set; }

        [Required(ErrorMessage = "ClientSecret is required.")]
        public string ClientSecret { get; set; }

        [Required(ErrorMessage = "Redirect uri is required.")]
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public CodeChallengeMethod? CodeChallengeMethod { get; set; }
        public string? CodeVerifier { get; set; }

        public string? Code { get; set; }
    }

    public enum CodeChallengeMethod
    {
        Plain = 0,
        S256
    }

    public class CodeChallengeMethodValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                // It's nullable, so it's valid if null
                return ValidationResult.Success;
            }

            if (Enum.IsDefined(typeof(CodeChallengeMethod), value))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid CodeChallengeMethod value.");
        }
    }

}
