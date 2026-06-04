using Microsoft.Extensions.DependencyInjection;

namespace AutoScheduler.DataAccess.Seeders
{
    public static class DataSeeder
    {
        public async static Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<SchedulerContext>();


        }
    }
}
