using Ecommerce.OpenAPI.Auth.Services;
using Microsoft.EntityFrameworkCore;


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

        public AdminController(UserRepository repository
            , IClientCredentialService clientCredentialService, ApplicationContext context, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
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
        [Route("client/assign")]
        public async Task<IActionResult> AssignClient([FromQuery] Guid userId)
        {
            EnsureDatabaseCreated(_context);
            var user = await _repository.GetById(userId);
            var client = await _clientCredentialService.InvokeCredentialsAsync();
            client.UserId = user.Id;
            await _context.OAuthClient.AddAsync(client);
            await _context.SaveChangesAsync();
            return ApiResponseExtension.ToSuccessApiResult(client);
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
