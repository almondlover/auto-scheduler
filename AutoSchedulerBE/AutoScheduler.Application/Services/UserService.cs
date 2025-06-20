using AutoScheduler.Domain.DTOs.Users;
using AutoScheduler.Domain.Entities.Users;
using AutoScheduler.Domain.Interfaces.Service;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IJWTService _jwtService;
        private readonly UserManager<User> _userManager;
        public UserService(UserManager<User> userManager, IJWTService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }
        public async Task<LoggedUserDTO> Login(LoginDTO loginDto)
        {
            string token = "";

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user!=null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
                token = _jwtService.GenerateJWTToken(user);
            var loggedUserData = new LoggedUserDTO() { User=user, Token = token};

            return loggedUserData;
        }
    }
}
