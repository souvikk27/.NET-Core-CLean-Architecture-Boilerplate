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
        [Required(ErrorMessage = "Client Id is Required")]
        public string Client_ID { get; set; }

        [Required(ErrorMessage = "Client Secret is Required")]
        public string Client_Secret { get; set; }

        [Required(ErrorMessage = "Refresh Token is Required")]
        public string Refresh_Token { get; set; }
    }
}
