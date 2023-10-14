using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Domain.Entities;
using Ecommerce.LoggerService;
using Ecommerce.Service;
using Ecommerce.Service.Abstraction;
using Ecommerce.Service.Context;
using Ecommerce.Service.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Ecommerce.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<UserRepository>();
            services.AddScoped(typeof(IRepositoryOptions<>), typeof(RepositoryOptions<>));
            services.AddTransient<CategoryRepository>();
        }

        public static void ConfigureSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "",
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line
            });
        }

        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });


        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.ClaimsIdentity.UserIdClaimType = "UserId";
            }).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>{
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
            });
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<DataContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("SqlConnection")));

        public static void ConfigureEntityContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<EntityContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("SqlConnection")));

        public static void ConfigureLogging(this IServiceCollection services) => 
            services.AddSingleton<ILoggerManager, LoggerManager>();

        public static void ConfigureDbSeed(this IServiceCollection services) =>
            services.AddScoped<IContextSeed, ContextSeed>();
    }
}