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
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(UserRepository repository
            , IClientCredentialService clientCredentialService, ApplicationContext context, IHttpClientFactory httpClientFactory)
        {
            this.repository = repository;
            _clientCredentialService = clientCredentialService;
            _context = context;
            _httpClientFactory = httpClientFactory;
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
