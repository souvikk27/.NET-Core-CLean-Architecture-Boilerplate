using Ecommerce.Domain.Entities;
using Ecommerce.LoggerService;
using Ecommerce.Service.Context;
using Ecommerce.Service.Contract.Generators;


namespace Ecommerce.Service.Seeding
{
    public class ContextSeed : IContextSeed
    {
        private readonly ProductGenerator productGenerator;
        private readonly CategoryGenerator categoryGenerator;
        private readonly ApplicationContext context;
        private readonly ILoggerManager logger;
        public ContextSeed(ApplicationContext context, ILoggerManager logger)
        {
            productGenerator = new ProductGenerator();
            categoryGenerator = new CategoryGenerator();
            this.context = context; 
            this.logger = logger;
        }

        public void SeedCategories()
        {
            try
            {
                List<Category> categories = categoryGenerator.GenerateCategories();
                List<string> existingCategories = context.Category.Select(c => c.Name).ToList();
                List<Category> newCategory = categories
                .Where(c => !existingCategories.Contains(c.Name))
                .ToList();
                if (newCategory.Count > 0)
                {
                    context.AddRange(newCategory);
                    context.SaveChanges();
                    logger.LogInformation("Categories Seeded to Database");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Unable to Seed Categories " + ex.Message);
                throw;
            }
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
            catch (Exception ex)
            {
                logger.LogError("Unable to Seed Products " + ex.Message);
                throw;
            }
            
        }
    }
}
