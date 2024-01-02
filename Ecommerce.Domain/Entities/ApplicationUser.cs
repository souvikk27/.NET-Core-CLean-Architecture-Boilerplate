using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [JsonIgnore]
        public ICollection<OAuthClient> OAuthClient { get; set; }
        [JsonIgnore]
        public ICollection<OrderDetails>? Orders { get; set; }
        [JsonIgnore]
        public ICollection<ShoppingSession>? ShoppingSessions { get; set; }
    }

}