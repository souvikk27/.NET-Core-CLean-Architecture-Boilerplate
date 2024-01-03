using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.DTO
{
    public class ClientDto
    {
        [Required(ErrorMessage = "Client Id is required")]
        public string Clientid { get; set; }

        [Required(ErrorMessage = "Client Secret is required")]
        public string Clientsecret { get; set; }
    }
}
