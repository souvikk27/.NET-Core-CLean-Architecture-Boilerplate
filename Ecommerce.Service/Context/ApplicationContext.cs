using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore;

namespace Ecommerce.Service.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options)
        : base(options)
        {
        }
    }
}