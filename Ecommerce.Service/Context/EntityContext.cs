using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service.Context
{
    public class EntityContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        {
        }

        public EntityContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Product> Product { get; set; }

        public virtual DbSet<Category> Category { get; set; }
    }
}
