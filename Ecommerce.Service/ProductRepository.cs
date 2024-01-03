using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Service.Abstraction;
using Ecommerce.Service.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Service
{
    public class ProductRepository : RepositoryBase<Product, ApplicationContext>,IProductRepository
    {
        public ProductRepository(IRepositoryOptions<ApplicationContext> options) : base(options)
        {
        }

        public override Expression<Func<ApplicationContext, DbSet<Product>>> DataSet() => o => o.Product;
        public override Expression<Func<Product, object>> Key() => o => o.Id;
    }
}