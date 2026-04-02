using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Application.Interfaces.Services;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Infrastructure.Data;
using DeviceManagement.Infrastructure.Data.Seed;
using DeviceManagement.Infrastructure.Repositories;
using DeviceManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDeviceManagementInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("A SQL Server connection string is required.", nameof(connectionString));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
