using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistance
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("ApplicationContext"));
                options.EnableSensitiveDataLogging(true).EnableDetailedErrors(true);
            });
            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                try
                {
                    var backendContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    if (backendContext.Database.GetPendingMigrations().Any())
                    {
                        backendContext.Database.Migrate();
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception($"Service Provider error: {ex}");
                }
            };

            return services;
        }
    }
}
