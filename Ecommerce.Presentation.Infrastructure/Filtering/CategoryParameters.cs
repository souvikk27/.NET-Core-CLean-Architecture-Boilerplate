using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Presentation.Infrastructure.Filtering
{
    public class CategoryParameters : RequestParameters
    {
        public string? CategoryName { get; set; }
        public DateTime AddedOn { get; set; }

    }
}
