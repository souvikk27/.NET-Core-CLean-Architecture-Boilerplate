using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class OAuthClient
    {
        [Key]
        public Guid Id { get; set; }

        public string Clientid { get; set; }

        public string Clientsecret { get; set; }

        public string? RefreshToken { get; set; }
        
        public DateTime? RefreshTokenExpiry { get; set; }

        public DateTime AddedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool isActive { get; set; }

        public string UserId { get; set; }

        // Navigation property
        public ApplicationUser User { get; set; }

    }
}
