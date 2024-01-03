using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public ICollection<UserAddress>? UserAddresses { get; set; }
        [JsonIgnore]
        public ICollection<UserPayment>? UserPayments { get; set; }
    }

}