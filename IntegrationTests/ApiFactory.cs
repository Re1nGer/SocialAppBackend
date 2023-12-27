using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistance;
using SocialApp.Services;

namespace IntegrationTests;

public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            //remove existing Database service

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // inject new testing database 
            //inject connection string with auth creds before running tests

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

            var connectionString = configuration.GetConnectionString("ApplicationContext");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var backendContext = sp.GetRequiredService<ApplicationDbContext>();

                backendContext.Database.EnsureCreated();
            }
        });
    }
    
    protected override void Dispose(bool disposing)
    {
        using var scope = Services.CreateScope();
        var sp = scope.ServiceProvider;
        var ctx = sp.GetRequiredService<ApplicationDbContext>();
        ctx.Database.EnsureDeleted();
    }
    
    public string GenerateJwt()
    {
        return JwtService.GenerateJwtToken(5);
    }
}