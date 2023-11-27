using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore;

namespace Ecommerce.Service.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) { }
    }
}