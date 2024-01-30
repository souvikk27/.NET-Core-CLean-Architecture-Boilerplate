using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
    public class CategoryWithChildren
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public List<CategoryWithChildren> Children { get; set; }
    }
}
