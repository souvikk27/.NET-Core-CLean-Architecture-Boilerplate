using Ecommerce.Service.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service
{
    public class RepositoryOptions<TContext> : IRepositoryOptions<TContext> where TContext : DbContext
    {
        private readonly TContext _context;

        public RepositoryOptions(TContext context)
        {
            _context = context;
        }

        public TContext Context => _context;
    }
}
