using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Presentation.Infrastructure.Filtering
{
    public class AuthParameters
    {
        [Required(ErrorMessage = "Grant Type is required")]
        public string grant_type  { get; set; }
        
        [Required(ErrorMessage = "Client Id is Required")]
        public string client_ID { get; set; }

        [Required(ErrorMessage = "Client Secret is Required")]
        public string client_Secret { get; set; }
        
        public string? refresh_Token { get; set; }
    }
}
