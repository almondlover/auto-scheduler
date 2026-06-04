using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AutoScheduler.DataAccess.Seeders
{
    public static class IdentitySeeder
    {
        static readonly string[] roleNames = ["Admin", "ResourceManager", "ScheduleManager"];
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            foreach (var roleName in roleNames) 
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
