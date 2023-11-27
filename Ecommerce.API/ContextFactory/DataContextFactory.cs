using Ecommerce.Service.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ecommerce.API.ContextFactory
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseSqlServer(configuration.GetConnectionString("SqlConnection"),
                b => b.MigrationsAssembly("Ecommerce.API"));

            return new DataContext(builder.Options);
        }
    }

    public class EntityContextFactory : IDesignTimeDbContextFactory<EntityContext>
    {
        public EntityContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<EntityContext>();
            builder.UseSqlServer(configuration.GetConnectionString("SqlConnection"),
                b => b.MigrationsAssembly("Ecommerce.API"));

            return new EntityContext(builder.Options);
        }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlServer(configuration.GetConnectionString("SqlConnection"),
                b => b.MigrationsAssembly("Ecommerce.API"));
            builder.UseOpenIddict();

            return new ApplicationDbContext(builder.Options);
        }
    } 
    
}
