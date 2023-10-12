using Ecommerce.Presentation.Extensions;
using Ecommerce.Service;
using Ecommerce.Service.Extensions;
using Ecommerce.Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserRepository repository;
        public AdminController(UserRepository repository)
        {
            this.repository = repository;
            
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserDto dto)
        {
            var user = dto.MaptoUser();
            var response = await repository.CreateUser(user, dto.Password);
            if(string.IsNullOrEmpty(response.UserName))
            {
                return ApiResponseExtension.ToErrorApiResult(dto,"Unable to create user");
            }
            return ApiResponseExtension.ToSuccessApiResult(response);
        }

    }
}
