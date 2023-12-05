
using Ecommerce.Service.Context;

namespace Ecommerce.API
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public Worker(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            await context.Database.EnsureCreatedAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
