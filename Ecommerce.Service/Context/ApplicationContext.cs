using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;
using System.Reflection.Emit;

namespace Ecommerce.Service.Context;

public class ApplicationContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public virtual DbSet<Product> Product { get; set; }

    public virtual DbSet<Category> Category { get; set; }

    public DbSet<ApplicationUser> User { get; set; }

    public virtual DbSet<OAuthClient> OAuthClient { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


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

        builder.Entity<OAuthClient>()
           .HasOne(o => o.User)
           .WithMany(u => u.OAuthClient)
           .HasForeignKey(o => o.UserId)
           .IsRequired();
    }
    
}