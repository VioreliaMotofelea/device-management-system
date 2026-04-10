using DeviceManagement.Infrastructure.Data;
using DeviceManagement.Infrastructure.Data.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceManagement.Tests.Integration.Fixtures;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"integration-tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbContextOptions = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbContextOptions is not null)
                services.Remove(dbContextOptions);

            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_dbName));

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            seeder.SeedAsync().GetAwaiter().GetResult();
        });
    }
}
