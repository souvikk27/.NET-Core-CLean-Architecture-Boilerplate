using System;
using Ecommerce.Presentation.Infrastructure.Filtering;
using Ecommerce.Presentation.Infrastructure.Services.Abstraction;
using Ecommerce.Service.Context;
using Ecommerce.Shared.DTO.Users;


namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserRepository repository;
        private readonly IClientCredentialService _clientCredentialService;
        private readonly ApplicationContext _context;
        private static bool _databaseChecked;

        public AdminController(UserRepository repository
            , IClientCredentialService clientCredentialService, ApplicationContext context)
        {
            this.repository = repository;
            _clientCredentialService = clientCredentialService;
            _context = context;
        }

        [HttpPost]
        [Route("register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> AddUser([FromBody] UserDto dto)
        {
            EnsureDatabaseCreated(_context);
            var user = dto.Adapt<ApplicationUser>();
            var cancellationToken = new CancellationTokenSource().Token;
            if (user == null) return ApiResponseExtension.ToErrorApiResult(dto, "User parameters required");

            var invokeClient = await _clientCredentialService.InvokeCredentialsAsync();

            var response = await repository.CreateUser(user, dto.Password, invokeClient);
            return ApiResponseExtension.ToSuccessApiResult(user);
        }

        [HttpPost]
        [Route("token")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> GenerateToken([FromForm] UserLoginDto userLoginDto)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7219");
            
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", userLoginDto.GrantType),
                new KeyValuePair<string, string>("username", userLoginDto.UserName),
                new KeyValuePair<string, string>("password", userLoginDto.Password),
                new KeyValuePair<string, string>("scope", userLoginDto.Scope)
                // Add other required parameters such as client_id, client_secret, scope, etc.
            });
            var response = await client.PostAsync("connect/token", formContent);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return ApiResponseExtension.ToErrorApiResult(errorMessage);
            }
            var tokenResponse = await response.Content.ReadAsStringAsync();
            return ApiResponseExtension.ToSuccessApiResult(tokenResponse);
        }
        
        
        

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await repository.GetAll();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await repository.GetById(id);
            return ApiResponseExtension.ToSuccessApiResult(user);
        }
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await repository.Delete(id);
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
