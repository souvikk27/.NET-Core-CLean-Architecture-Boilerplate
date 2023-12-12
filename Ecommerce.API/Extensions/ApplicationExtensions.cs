namespace Ecommerce.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static WebApplication ConfigureDatabaseSeed(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var contextSeed = services.GetRequiredService<IContextSeed>();
            contextSeed.SeedProducts();
            contextSeed.SeedCategories();
            return app;
        }
    }
}
