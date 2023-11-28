using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace Ecommerce.Service.Context
{
    public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.HasDefaultSchema("Identity");


            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");

            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            builder.Entity<OpenIddictEntityFrameworkCoreToken<string>>(entity => entity.ToTable("OpenIddictTokens")); 

            builder.Entity<OpenIddictEntityFrameworkCoreApplication<string>>(entity => entity.ToTable("OpenIddictApplications"));

            builder.Entity<OpenIddictEntityFrameworkCoreAuthorization<string>>(entity => entity.ToTable("OpenIddictAuthorizations"));

            builder.Entity<OpenIddictEntityFrameworkCoreScope<string>>(entity => entity.ToTable("OpenIddictScopes"));

        }
    }
}