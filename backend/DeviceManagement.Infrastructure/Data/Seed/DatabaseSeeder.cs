using DeviceManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Data.Seed;

public sealed class DatabaseSeeder
{
    public const string DemoUserEmail = "test@user.com";
    public const string DemoUserPassword = "Password1";
    private const string SharedSeedPassword = DemoUserPassword;

    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public DatabaseSeeder(AppDbContext db, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var usersByEmail = await SeedUsersAsync(cancellationToken);
        await SeedDevicesAsync(usersByEmail, cancellationToken);
    }

    private async Task<Dictionary<string, User>> SeedUsersAsync(CancellationToken cancellationToken)
    {
        var seeds = new[]
        {
            new UserSeed(DemoUserEmail, "Test User", "Employee", "London"),
            new UserSeed("alex.williams@mmc.local", "Alex Williams", "Employee", "London"),
            new UserSeed("maria.popescu@mmc.local", "Maria Popescu", "Employee", "Bucharest"),
            new UserSeed("daniel.ionescu@mmc.local", "Daniel Ionescu", "Employee", "Bucharest"),
            new UserSeed("fatima.khan@mmc.local", "Fatima Khan", "Manager", "Berlin"),
            new UserSeed("jonas.mueller@mmc.local", "Jonas Mueller", "Employee", "Berlin"),
            new UserSeed("sofie.larsen@mmc.local", "Sofie Larsen", "Employee", "Copenhagen"),
            new UserSeed("li.wei@mmc.local", "Li Wei", "Employee", "Singapore")
        };

        var existingUsers = await _db.Users.ToListAsync(cancellationToken);
        var usersByEmail = existingUsers.ToDictionary(u => u.Email, StringComparer.OrdinalIgnoreCase);

        foreach (var seed in seeds)
        {
            if (!usersByEmail.TryGetValue(seed.Email, out var user))
            {
                user = new User
                {
                    Email = seed.Email
                };
                _db.Users.Add(user);
                usersByEmail[seed.Email] = user;
            }

            user.FullName = seed.FullName;
            user.Role = seed.Role;
            user.Location = seed.Location;

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, SharedSeedPassword);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return usersByEmail;
    }

    private async Task SeedDevicesAsync(
        IReadOnlyDictionary<string, User> usersByEmail,
        CancellationToken cancellationToken)
    {
        var seeds = new[]
        {
            new DeviceSeed("iPhone 14", "Apple", "phone", "iOS", "16.7", "A15 Bionic", "6 GB", "Reliable iPhone for office productivity and standard business communication.", "London", DemoUserEmail),
            new DeviceSeed("iPhone 15 Pro", "Apple", "phone", "iOS", "17.5", "A17 Pro", "8 GB", "High-end iPhone for heavy multitasking and mobile executive workflows.", "London", "alex.williams@mmc.local"),
            new DeviceSeed("Galaxy S23", "Samsung", "phone", "Android", "14", "Snapdragon 8 Gen 2", "8 GB", "Balanced Android flagship with strong performance and battery life.", "Berlin", "jonas.mueller@mmc.local"),
            new DeviceSeed("Galaxy A54", "Samsung", "phone", "Android", "14", "Exynos 1380", "6 GB", "Mid-range Android phone suitable for daily field operations.", "Bucharest", null),
            new DeviceSeed("Pixel 8", "Google", "phone", "Android", "14", "Google Tensor G3", "8 GB", "Clean Android experience used for QA and app compatibility checks.", "Copenhagen", "sofie.larsen@mmc.local"),
            new DeviceSeed("Xiaomi 13", "Xiaomi", "phone", "Android", "14", "Snapdragon 8 Gen 2", "8 GB", "Cost-effective high-performance Android device for support teams.", "Bucharest", null),
            new DeviceSeed("Mate 50", "Huawei", "phone", "HarmonyOS", "4", "Kirin 9000S", "8 GB", "Huawei phone used for regional testing and vendor-specific support.", "Singapore", "li.wei@mmc.local"),
            new DeviceSeed("iPad Air 5", "Apple", "tablet", "iPadOS", "17", "Apple M1", "8 GB", "Portable tablet for mobile presentations and executive travel use.", "London", "fatima.khan@mmc.local"),
            new DeviceSeed("iPad 10th Gen", "Apple", "tablet", "iPadOS", "17", "A14 Bionic", "4 GB", "General-purpose tablet used in meeting rooms and onboarding.", "London", null),
            new DeviceSeed("Galaxy Tab S9", "Samsung", "tablet", "Android", "14", "Snapdragon 8 Gen 2", "12 GB", "Premium Android tablet for design reviews and demonstration workflows.", "Berlin", null),
            new DeviceSeed("Lenovo Tab P12", "Lenovo", "tablet", "Android", "14", "MediaTek Dimensity 7050", "8 GB", "Large-screen collaboration tablet for documentation and remote sessions.", "Bucharest", "maria.popescu@mmc.local"),
            new DeviceSeed("Surface Go 3 LTE", "Microsoft", "tablet", "Windows", "11", "Intel Pentium Gold 6500Y", "8 GB", "Windows tablet with LTE for hybrid staff and travel scenarios.", "Copenhagen", "daniel.ionescu@mmc.local")
        };

        var existingDevices = await _db.Devices.ToListAsync(cancellationToken);
        var devicesByKey = existingDevices.ToDictionary(
            d => MakeDeviceKey(d.Name, d.Manufacturer),
            StringComparer.OrdinalIgnoreCase);

        foreach (var seed in seeds)
        {
            var key = MakeDeviceKey(seed.Name, seed.Manufacturer);
            if (!devicesByKey.TryGetValue(key, out var device))
            {
                device = new Device
                {
                    Name = seed.Name,
                    Manufacturer = seed.Manufacturer
                };
                _db.Devices.Add(device);
                devicesByKey[key] = device;
            }

            device.Type = seed.Type;
            device.OperatingSystem = seed.OperatingSystem;
            device.OsVersion = seed.OsVersion;
            device.Processor = seed.Processor;
            device.RamAmount = seed.RamAmount;
            device.Description = seed.Description;
            device.Location = seed.Location;
            device.AssignedUserId = ResolveAssignedUserId(seed.AssignedUserEmail, usersByEmail);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private static int? ResolveAssignedUserId(
        string? email,
        IReadOnlyDictionary<string, User> usersByEmail)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        if (!usersByEmail.TryGetValue(email, out var user))
            return null;

        return user.Id;
    }

    private static string MakeDeviceKey(string name, string manufacturer)
        => $"{name.Trim().ToLowerInvariant()}::{manufacturer.Trim().ToLowerInvariant()}";

    private sealed record UserSeed(string Email, string FullName, string Role, string Location);

    private sealed record DeviceSeed(
        string Name,
        string Manufacturer,
        string Type,
        string OperatingSystem,
        string OsVersion,
        string Processor,
        string RamAmount,
        string Description,
        string Location,
        string? AssignedUserEmail);
}
