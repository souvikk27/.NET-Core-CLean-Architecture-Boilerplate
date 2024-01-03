using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.OpenAPI.Auth.AuthEntity
{
    public class Consts
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public const string ConsentNaming = "consent";
        public const string GrantAccessValue = "Grant";
        public const string DenyAccessValue = "Deny";
    }
}