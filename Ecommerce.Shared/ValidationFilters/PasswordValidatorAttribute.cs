using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ecommerce.Shared.DTO
{
    public class PasswordValidatorAttribute : ValidationAttribute
    {
        private int minLength;
        public PasswordValidatorAttribute(int minLength)
        {
            this.minLength = minLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;
            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password is required");
            }

            if (password.Length < minLength)
            {
                return new ValidationResult($"Minimum Length for Password should be {minLength}");
            }

            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$"))
            {
                return new ValidationResult("Password must contain at least one lowercase letter, one uppercase letter, and one digit.");
            }
            return ValidationResult.Success;
        }
    }
}