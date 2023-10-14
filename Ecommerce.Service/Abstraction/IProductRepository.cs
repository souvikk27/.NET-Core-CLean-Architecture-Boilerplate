using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service.Abstraction
{
    public interface IProductRepository : IRepository<Product>
    {
        void Save();
    }
}
