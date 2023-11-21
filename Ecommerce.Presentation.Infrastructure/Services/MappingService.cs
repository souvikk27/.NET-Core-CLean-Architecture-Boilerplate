using Ecommerce.Domain.Entities;
using Ecommerce.Shared.DTO;
using Mapster;

namespace Ecommerce.Presentation.Infrastructure.Services
{
    public static class MappingService
    {
        public static void InitializeMapping()
        {
            CreateUserMapping();
        }


        public static void CreateUserMapping()
        {
            TypeAdapterConfig<UserDto, ApplicationUser>.NewConfig().Compile();
        }

        public static void CreateProductMapping()
        {
            TypeAdapterConfig<ProductDto, Product>.NewConfig().Compile();
        }

        public static void CreateCategpryMapping()
        {
            TypeAdapterConfig<CategoryDto , Category>.NewConfig().Compile();
        }
    }
}
