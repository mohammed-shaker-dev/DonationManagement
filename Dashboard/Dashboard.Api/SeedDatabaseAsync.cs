using Dashboard.Infrastructure.Data;

namespace Dashboard.Api
{
    public static class WebApplicationExtensions
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var hostEnvironment = services.GetService<IWebHostEnvironment>();
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogInformation($"Starting in environment {hostEnvironment.EnvironmentName}");
                try
                {
                    var seedService = services.GetRequiredService<AppDbContextSeed>();
                    await seedService.SeedAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
        }
    }
}
