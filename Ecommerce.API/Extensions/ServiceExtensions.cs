using Ecommerce.Domain.Entities;
using Ecommerce.Presentation.Infrastructure.Services;
using Ecommerce.Presentation.Infrastructure.Services.Abstraction;
using Ecommerce.Service;
using Ecommerce.Service.Context;
using Ecommerce.Service.Contract.Generators;
using Ecommerce.Service.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;


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
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line is needed because of the following code in the assembly
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative),
                            TokenUrl = new Uri("/connect/token", UriKind.Relative),
                            Scopes = new Dictionary<string, string>
                            {
                                {"openid", "OpenID"},
                                {"profile", "Profile"},
                            }
                        }
                    }
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "openid"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        public static void ConfigureOpenIddict(this IServiceCollection services)
        {
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<ApplicationContext>();
                    options.UseQuartz();
                })
                .AddServer(options =>
                {
                    options.SetIssuer(new Uri("https://localhost/7129"));
                    options.SetAuthorizationEndpointUris("connect/authorize")
                        .SetLogoutEndpointUris("connect/logout")
                        .SetTokenEndpointUris("connect/token");
                    

                    options.AllowPasswordFlow()
                       .AllowRefreshTokenFlow();


                    options.AllowClientCredentialsFlow()
                    .AllowRefreshTokenFlow();

                    options.AcceptAnonymousClients();

                    options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                    options.DisableAccessTokenEncryption();
                    
                    options
                        .AddEphemeralEncryptionKey()
                        .AddEphemeralSigningKey();

                    options.RegisterScopes("api");

                    options.UseAspNetCore()
                        .EnableLogoutEndpointPassthrough()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough();
                })
                 .AddValidation(options =>
                 {
                     options.UseLocalServer();

                     options.UseAspNetCore();
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
            }).AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
            });
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<ApplicationContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
                option.UseOpenIddict();
            });

        public static void ConfigureLogging(this IServiceCollection services) => 
            services.AddSingleton<ILoggerManager, LoggerManager>();

        public static void ConfigureDbSeed(this IServiceCollection services) =>
            services.AddScoped<IContextSeed, ContextSeed>();

        public static void ConfigureTokenGeneration(this IServiceCollection services) =>
            services.AddSingleton<TokenGenerator>();

        public static void ConfigureHostedservice(this IServiceCollection services) => services.AddHostedService<Worker>();

        public static void ConfigureQuartz(this IServiceCollection services)
        {
            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        }

        public static void InvokeOauthClient(this IServiceCollection services) => 
            services.AddTransient<IClientCredentialService, ClientCredentialService>();

        //public static void TriggerOpenIdValidation(this IServiceCollection services) =>
        //    services.AddScoped<IOpenIdValidationService, OpenIdValidationService>();
    }
}