using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoScheduler.Domain.Entities.Users;

namespace AutoScheduler.Domain.DTOs.Users
{
    public class LoggedUserDTO
    {
        public required User User { get; set; }
        public required string Token { get; set;  }
    }
}
