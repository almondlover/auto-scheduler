using AutoScheduler.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Interfaces.Service
{
    public interface IJWTService
    {
        public string GenerateJWTToken(User user);
    }
}
