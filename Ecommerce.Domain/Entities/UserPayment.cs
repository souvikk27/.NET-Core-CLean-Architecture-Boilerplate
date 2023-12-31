using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class UserPayment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Payment Payment { get; set; }
    }
}
