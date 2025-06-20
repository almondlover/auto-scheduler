using AutoScheduler.Domain.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Interfaces.Service
{
    public interface IUserService
    {
        public Task<LoggedUserDTO> Login(LoginDTO loginDto);
    }
}
