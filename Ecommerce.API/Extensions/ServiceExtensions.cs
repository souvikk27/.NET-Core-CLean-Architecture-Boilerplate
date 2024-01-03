using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

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

        public static void InvokeAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                config.DefaultChallengeScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
                config.DefaultAuthenticateScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
            });
        }

        public static void ConfigureOpenIddict(this IServiceCollection services)
        {
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<ApplicationContext>();
                    options.UseQuartz()
                            .SetMinimumTokenLifespan(TimeSpan.FromHours(1))
                            .SetMaximumRefireCount(3);
                })
                .AddClient(options =>
                {
                    options.AllowAuthorizationCodeFlow();

                    options.UseAspNetCore()
                       .EnableStatusCodePagesIntegration()
                       .EnableRedirectionEndpointPassthrough();

                    options
                        .AddEphemeralEncryptionKey()
                        .AddEphemeralSigningKey();

                    options.UseSystemNetHttp().SetProductInformation(typeof(Program).Assembly);
                    options.UseWebProviders();
                    options.Configure(options =>
                    {
                        options.GrantTypes.Add(GrantTypes.AuthorizationCode);
                        options.RedirectionEndpointUris.Add(new Uri("https://localhost:7219/callback/login/local", UriKind.Absolute));
            
                        options.PostLogoutRedirectionEndpointUris.Add(new Uri("https://localhost:7219/callback/logout/local", UriKind.Absolute));
                    });

                    options.AddRegistration(new OpenIddictClientRegistration
                    {
                        Issuer = new Uri("https://localhost:7219/", UriKind.Absolute),

                        // Note: to mitigate mix-up attacks, it's recommended to use a unique redirection endpoint
                        // URI per provider, unless all the registered providers support returning a special "iss"
                        // parameter containing their URL as part of authorization responses. For more information,
                        // see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
                        RedirectUri = new Uri("callback/login/local", UriKind.Relative),
                        PostLogoutRedirectUri = new Uri("callback/logout/local", UriKind.Relative)
                    });

                })
                .AddServer(options =>
                {
                    options.SetIssuer(new Uri("https://localhost/7129"));

                    options.SetAuthorizationEndpointUris("connect/authorize")
                        .SetLogoutEndpointUris("connect/logout")
                        .SetIntrospectionEndpointUris("/connect/introspect")
                        .SetTokenEndpointUris("/connect/token")
                        .SetUserinfoEndpointUris("/connect/userinfo")
                        .SetVerificationEndpointUris("/connect/verify");
                    
                    options.AllowPasswordFlow()
                           .AllowRefreshTokenFlow()
                           .AllowHybridFlow()
                           .AllowClientCredentialsFlow()
                           .AllowAuthorizationCodeFlow();

                    options.SetRefreshTokenLifetime(null).DisableRollingRefreshTokens();

                    options.RequireProofKeyForCodeExchange();

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
                        .EnableStatusCodePagesIntegration()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableLogoutEndpointPassthrough()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableUserinfoEndpointPassthrough()
                        .EnableVerificationEndpointPassthrough();
                })
                 .AddValidation(options =>
                 {
                     options.UseLocalServer();
                     options.EnableAuthorizationEntryValidation();
                     options.UseAspNetCore();
                 });
        }

        public static void ConfigureStaticRednering(this IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddRazorComponents();
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

        public static void TriggerOpenIdAuthentication(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
        }
    }
}