﻿using Ecommerce.Domain.Entities;
using Ecommerce.Service.Abstraction;
using Ecommerce.Service.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service
{
    public class CategoryRepository : RepositoryBase<Category, ApplicationContext>
    {
        public CategoryRepository(IRepositoryOptions<ApplicationContext> options) : base(options)
        {
        }

        public override Expression<Func<ApplicationContext, DbSet<Category>>> DataSet() => o => o.Category;
        public override Expression<Func<Category, object>> Key() => o => o.Id;
    }
}
