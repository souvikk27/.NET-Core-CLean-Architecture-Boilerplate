using Ecommerce.OpenAPI.Auth.AuthEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Client.AspNetCore;
using RestSharp;
using static OpenIddict.Abstractions.OpenIddictConstants;


namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserRepository _repository;
        private readonly IClientCredentialService _clientCredentialService;
        private readonly ApplicationContext _context;
        private static bool _databaseChecked;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOpenIddictApplicationManager _applicationManager;

        public AdminController(UserRepository repository
            , IClientCredentialService clientCredentialService, ApplicationContext context, IHttpClientFactory httpClientFactory, IOpenIddictApplicationManager applicationManager, SignInManager<ApplicationUser> signInManager)
        {
            _repository = repository;
            _clientCredentialService = clientCredentialService;
            _context = context;
            _applicationManager = applicationManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> AddUser([FromBody] UserDto dto)
        {
            EnsureDatabaseCreated(_context);
            var user = dto.Adapt<ApplicationUser>();
            // var cancellationToken = new CancellationTokenSource().Token;
            if (user == null) return ApiResponseExtension.ToErrorApiResult(dto, "User parameters required");

            var invokeClient = await _clientCredentialService.InvokeCredentialsAsync();

            var response = await _repository.CreateUser(user, dto.Password, invokeClient);
            
            return ApiResponseExtension.ToSuccessApiResult(response);
        }

        [HttpGet]
        [Route("client")]
        public async Task<IActionResult> ConfigureClient([FromQuery] Guid userId)
        {
            EnsureDatabaseCreated(_context);
            var user = await _repository.GetById(userId);
            if(user == null) return ApiResponseExtension.ToErrorApiResult(userId, "User not found");

            var client = _context.OAuthClient.Where(client => client.UserId == user.Id && client.isActive).ToList();
            return ApiResponseExtension.ToSuccessApiResult(client);
        }

        [HttpPost]
        [Route("client/register")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> RegisterClient([FromForm] Guid userId)
        {
            EnsureDatabaseCreated(_context);
            var user = await _repository.GetById(userId);
            if (user == null) return ApiResponseExtension.ToErrorApiResult(userId, "User not found");
            var client = await _clientCredentialService.InvokeCredentialsAsync();
            client.UserId = user.Id;
            await _context.OAuthClient.AddAsync(client);
            await _context.SaveChangesAsync();
            return ApiResponseExtension.ToSuccessApiResult(client);
        }

        [HttpPost]
        [Route("client/assign")]
        public async Task<IActionResult> AssignClient([FromForm] ClientDto clientDto, CancellationToken cancellationToken)
        {
            var client = clientDto.Adapt<OAuthClient>();
            var applicationClient = await _applicationManager.FindByClientIdAsync(client.Clientid);
            if(applicationClient == null)
            {
                await _applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = client.Clientid,
                    ClientSecret = client.Clientsecret,
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "Ecommerce client application",
                    PostLogoutRedirectUris =
                    {
                        new Uri("https://localhost:7219/callback/logout/local")
                    },
                    RedirectUris =
                    {
                        new Uri("https://localhost:7219/callback/login/local")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Token,
                        Permissions.ResponseTypes.CodeIdToken,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                        Permissions.Prefixes.Scope + "api"
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                }, cancellationToken);

                return ApiResponseExtension.ToSuccessApiResult("OpenId Server Response", "Client assigned successfully");
            }
            return ApiResponseExtension.ToWarningApiResult("OpenId Server Response", "Client already exists");
        }
        
        [HttpPost]
        [Route("client/retire")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> RetireClient([FromForm] Guid clientId)
        {
            EnsureDatabaseCreated(_context);
            var client = await _context.OAuthClient.Where(client => client.Id == clientId).FirstOrDefaultAsync();
            client.isActive = false;
            await _context.SaveChangesAsync();
            return ApiResponseExtension.ToSuccessApiResult(client);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            var properties = new AuthenticationProperties
            {
                // Only allow local return URLs to prevent open redirect attacks.
                RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
            };
            // Ask the OpenIddict client middleware to redirect the user agent to the identity provider.
            return await Task.FromResult<IActionResult>(Challenge(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
        }

        [HttpPost]
        [Route("token")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> GenerateToken([FromForm] UserLoginDto userLoginDto)
        {
            var username = userLoginDto.UserName;
            var password = userLoginDto.Password;
            var scope = userLoginDto.Scope;

            // Create a new HttpClient instance
            var client = _httpClientFactory.CreateClient();

            // Prepare the form data
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("scope", scope)
            };

            // Create a FormUrlEncodedContent instance
            var content = new FormUrlEncodedContent(formData);

            // Make the POST request to the Exchange endpoint
            var response = await client.PostAsync("https://localhost:7219/connect/token", content);

            // Handle the response
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                // Parse and handle the response data as needed
                return Ok(responseData);
            }
            else
            {
                // Handle the error response
                var errorResponse = await response.Content.ReadAsStringAsync();
                return BadRequest(errorResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repository.GetAll();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _repository.GetById(id);
            return ApiResponseExtension.ToSuccessApiResult(user);
        }
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _repository.Delete(id);
            return ApiResponseExtension.ToSuccessApiResult(user, "User credentials removed");
        }

        private static void EnsureDatabaseCreated(ApplicationContext context)
        {
            if (_databaseChecked) return;
            _databaseChecked = true;
            context.Database.EnsureCreated();
        }
    }
}
