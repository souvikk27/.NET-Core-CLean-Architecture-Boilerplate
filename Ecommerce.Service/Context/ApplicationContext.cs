using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

        builder.Entity<ProductCategory>()
        .HasKey(pc => new { pc.ProductId, pc.CategoryId });

        builder.Entity<ProductCategory>()
            .HasOne(pc => pc.Product)
            .WithMany(p => p.ProductCategories)
            .HasForeignKey(pc => pc.ProductId);

        builder.Entity<ProductCategory>()
            .HasOne(pc => pc.Category)
            .WithMany(c => c.ProductCategories)
            .HasForeignKey(pc => pc.CategoryId);

        builder.Entity<Product>()
            .HasOne(p => p.Discount)
            .WithMany()
            .HasForeignKey(p => p.DiscountId);

        builder.Entity<Product>()
            .HasOne(p => p.Inventory)
            .WithMany()
            .HasForeignKey(p => p.InventoryId);

        builder.Entity<OrderDetails>()
            .HasOne(od => od.Payment)
            .WithOne(p => p.OrderDetails)
            .HasForeignKey<OrderDetails>(od => od.PaymentId);

        builder.Entity<OrderDetails>()
            .HasOne(od => od.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(od => od.UserId);

        builder.Entity<OrderDetails>()
            .HasOne(od => od.OrderPayment)
            .WithOne(op => op.OrderDetails)
            .HasForeignKey<OrderPayment>(op => op.Orderid);

        builder.Entity<ShoppingSession>()
            .HasOne(ss => ss.User)
            .WithMany(u => u.ShoppingSessions)
            .HasForeignKey(ss => ss.UserId);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.ShoppingSessions)
            .WithOne(ss => ss.User)
            .HasForeignKey(ss => ss.UserId);

        builder.Entity<CartProduct>()
        .HasKey(cp => new { cp.CartId, cp.ProductId });
    }
    
}