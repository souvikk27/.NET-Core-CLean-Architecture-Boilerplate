using Microsoft.Extensions.DependencyInjection;
using Mapster;
using Ecommerce.Shared.DTO;
using Ecommerce.Domain.Entities;
using Ecommerce.Presentation.Infrastructure.Services;

namespace Ecommerce.Presentation.Infrastructure.Extensions
{
    public static class MappingServiceExtension
    {
        public static IServiceCollection AddMappingConfig(this IServiceCollection services)
        {
            MappingService.InitializeMapping();
            return services;
        }
    }
}
