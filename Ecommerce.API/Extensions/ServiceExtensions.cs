using System.Text;
using Ecommerce.Domain.Entities;
using Ecommerce.LoggerService;
using Ecommerce.Service;
using Ecommerce.Service.Abstraction;
using Ecommerce.Service.Context;
using Ecommerce.Service.Contract.Generators;
using Ecommerce.Service.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;

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



        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
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
            {
                option.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
                option.UseOpenIddict();
            });




        public static void ConfigureEntityContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<EntityContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("SqlConnection")));




        public static void ConfigureLogging(this IServiceCollection services) => 
            services.AddSingleton<ILoggerManager, LoggerManager>();





        public static void ConfigureDbSeed(this IServiceCollection services) =>
            services.AddScoped<IContextSeed, ContextSeed>();





        public static void ConfigureTokenGeneration(this IServiceCollection services) =>
            services.AddSingleton<TokenGenerator>();



        public static void COnfigureOpeniddict(this IServiceCollection services) =>
            services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore().UseDbContext<DataContext>(); // Replace with your DbContext>
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/connect/authorize")
                       .SetTokenEndpointUris("/connect/token");

                options.AllowAuthorizationCodeFlow()
                       .AllowPasswordFlow()
                       .AllowRefreshTokenFlow();

                options.AddEphemeralEncryptionKey()
                       .AddEphemeralSigningKey()
                       .DisableAccessTokenEncryption();

                options.RegisterScopes(OpenIddictConstants.Scopes.Email,
                                       OpenIddictConstants.Scopes.Profile,
                                       OpenIddictConstants.Scopes.Roles);

                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableTokenEndpointPassthrough();
            });


    }
}