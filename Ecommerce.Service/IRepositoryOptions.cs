using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service
{
    public interface IRepositoryOptions<TContext> where TContext : DbContext
    {
        TContext Context { get; }
    }
}
