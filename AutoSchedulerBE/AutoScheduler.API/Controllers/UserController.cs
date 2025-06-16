using AutoScheduler.Domain.DTOs.User;
using AutoScheduler.Domain.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoScheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("user/login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var token = await _userService.Login(loginDto);

            if (token != "") return Ok(token);
            return Unauthorized();
        }
    }
}
