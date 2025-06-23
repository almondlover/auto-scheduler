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
        public async Task<LoggedUserDTO> LoginAsync(LoginDTO loginDto)
        {
            string token = "";

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            //todo: use all roles for claims/check most privileged?
            var userRole = (await _userManager.GetRolesAsync(user))[0];
            if (user!=null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
                token = _jwtService.GenerateJWTToken(user, userRole);
            var loggedUserData = new LoggedUserDTO() { User=user, Token = token};

            return loggedUserData;
        }

        public async Task RegisterAsync(RegisterDTO registerDTO)
        {
            var newUser = new User { UserName = registerDTO.UserName, Email = registerDTO.Email };
            var result = await _userManager.CreateAsync(newUser, registerDTO.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(newUser, registerDTO.Role);
        }
    }
}
