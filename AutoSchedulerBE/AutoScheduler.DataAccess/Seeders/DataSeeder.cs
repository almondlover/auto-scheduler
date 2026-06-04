using Microsoft.Extensions.DependencyInjection;

namespace AutoScheduler.DataAccess.Seeders
{
    public static class DataSeeder
    {
        public async static Task SeedFromCsvAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<SchedulerContext>();

            await RequirementSeeder.SeedFromCsvAsync(context);

            await HallSeeder.SeedFromCsvAsync(context);


        }
    }
}
