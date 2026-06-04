using AutoScheduler.DataAccess.Seeders.Models;
using AutoScheduler.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AutoScheduler.DataAccess.Seeders
{
    public static class IdentitySeeder
    {
        static readonly string[] roleNames = ["Admin", "ResourceManager", "ScheduleManager"];
        static readonly UserSeed[] testUsers = [
            new UserSeed {
                Username = "TestAdmin",
                Password = "TestPassword123!",
                Email = "testadmin@mail.com",
                Role = "Admin"
            },
            new UserSeed {
                Username = "TestManager",
                Password = "TestPassword123!",
                Email = "testmng@mail.com",
                Role = "ResourceManager"
            },
            new UserSeed {
                Username = "TestScheduler",
                Password = "TestPassword123!",
                Email = "testsched@mail.com",
                Role = "ScheduleManager"
            },
        ];
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            foreach (var roleName in roleNames) 
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        public static async Task SeedTestUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            foreach (var user in testUsers)
            {
                if (await userManager.FindByEmailAsync(user.Email) == null)
                {
                    var newUser = new User
                    {
                        UserName = user.Username,
                        Email = user.Email
                    };
                    
                    await userManager.CreateAsync(newUser, user.Password);
                    await userManager.AddToRoleAsync(newUser, user.Role);
                }
            }
        }
    }
}
