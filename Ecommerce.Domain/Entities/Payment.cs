using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }
        public string PaymentType { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string AccountNo { get; set; }
        public string Expiry { get; set; }
        public OrderDetails OrderDetails { get; set; }
    }
}
