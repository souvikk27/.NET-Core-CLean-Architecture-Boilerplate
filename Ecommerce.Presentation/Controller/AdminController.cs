using Ecommerce.Presentation.ActionFilters;
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
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> AddUser(UserDto dto)
        {
            var user = dto.MaptoUser();
            var response = await repository.CreateUser(user, dto.Password);
            if(response == null)
            {
                return ApiResponseExtension.ToErrorApiResult(dto.UserName, "User Already Exists");
            }
            return ApiResponseExtension.ToSuccessApiResult(response);
        }

    }
}
