using Ecommerce.Domain.Entities;
using Ecommerce.LoggerService;
using Ecommerce.Service.Context;
using Ecommerce.Service.Contract.Generators;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service.Seeding
{
    public class ContextSeed : IContextSeed
    {
        private readonly ProductGenerator productGenerator;
        private readonly EntityContext context;
        private readonly ILoggerManager logger;
        public ContextSeed(EntityContext context, ILoggerManager logger)
        {
            productGenerator = new ProductGenerator();
            this.context = context; 
            this.logger = logger;
        }

        public void SeedProducts()
        {
            try
            {
                List<Product> products = productGenerator.GenerateProducts();
                List<string> existingProductSKUs = context.Product.Select(p => p.SKU).ToList();

                List<Product> newProducts = products
                .Where(p => !existingProductSKUs.Contains(p.SKU))
                .ToList();
                if(newProducts.Count > 0)
                {
                    context.AddRange(newProducts);
                    context.SaveChanges();
                    logger.LogInformation("Products Seeded to Database");
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
