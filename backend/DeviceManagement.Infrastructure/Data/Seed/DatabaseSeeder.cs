using DeviceManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Data.Seed;

public sealed class DatabaseSeeder
{
    public const string DemoUserEmail = "test@user.com";
    private const string DemoUserPassword = "Password1";
    private const string DemoDeviceName = "iPhone 14";

    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public DatabaseSeeder(AppDbContext db, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedDemoUserAsync(cancellationToken);
        await SeedDemoDeviceAsync(cancellationToken);
    }

    private async Task SeedDemoUserAsync(CancellationToken cancellationToken)
    {
        var exists = await _db.Users.AsNoTracking().AnyAsync(u => u.Email == DemoUserEmail, cancellationToken);
        if (exists)
            return;

        var user = new User
        {
            Email = DemoUserEmail,
            FullName = "Test User",
            Role = "Employee",
            Location = "London"
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, DemoUserPassword);

        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedDemoDeviceAsync(CancellationToken cancellationToken)
    {
        var exists = await _db.Devices.AsNoTracking().AnyAsync(d => d.Name == DemoDeviceName, cancellationToken);
        if (exists)
            return;

        await _db.Devices.AddAsync(
            new Device
            {
                Name = DemoDeviceName,
                Manufacturer = "Apple",
                Type = "phone",
                OperatingSystem = "iOS",
                OsVersion = "16",
                Processor = "A15",
                RamAmount = "6GB",
                Description = "Apple smartphone with A15 chip, 6GB RAM, iOS 16.",
                Location = "London"
            },
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
