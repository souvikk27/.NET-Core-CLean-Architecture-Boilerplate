using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Service.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Service
{
    public class ProductRepository : RepositoryBase<Product, EntityContext>
    {
        public ProductRepository(IRepositoryOptions<EntityContext> options) : base(options)
        {
        }

        public override Expression<Func<EntityContext, DbSet<Product>>> DataSet() => o => o.Product;
        public override Expression<Func<Product, object>> Key() => o => o.Id;
    }
}