using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Presentation.Infrastructure.Filtering
{
    public class ProductsParameters : RequestParameters
    {
        public decimal MaxPrice { get; set; } = decimal.MaxValue;
        public decimal MinPrice { get; set; } = 0;
        public bool ValidatePriceRange => MaxPrice > MinPrice;
        public DateTime AddedOn { get; set; }
        public string? Sku { get; set; }
    }
}
