using System;
using Ecommerce.Presentation.Infrastructure.Filtering;
using Ecommerce.Presentation.Infrastructure.Services.Abstraction;
using Ecommerce.Service.Context;


namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
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
