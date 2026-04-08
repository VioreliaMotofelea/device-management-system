using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DeviceManagement.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceManagement.Tests.Integration.Infrastructure;

public sealed class DeviceRepositoryIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeviceRepositoryIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetByNameAndManufacturerAsync_ReturnsDevice_WhenExists()
    {
        using var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "repo-hit");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var name = $"repo-{Guid.NewGuid():N}".Substring(0, 12);
        const string manufacturer = "RepoBrand";

        var create = await client.PostAsJsonAsync("/api/devices", new
        {
            Name = name,
            Manufacturer = manufacturer,
            Type = "phone",
            OperatingSystem = "Android",
            OsVersion = "14",
            Processor = "Mid Chip",
            RamAmount = "8 GB",
            Description = "Repository integration test device.",
            Location = "London"
        });
        create.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
        var device = await repo.GetByNameAndManufacturerAsync(name, manufacturer);

        Assert.NotNull(device);
        Assert.Equal(name, device!.Name);
        Assert.Equal(manufacturer, device.Manufacturer);
    }

    [Fact]
    public async Task GetByNameAndManufacturerAsync_ReturnsNull_WhenMissing()
    {
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();

        var device = await repo.GetByNameAndManufacturerAsync($"missing-{Guid.NewGuid():N}", "NoBrand");

        Assert.Null(device);
    }

    private static async Task<string> RegisterAndLoginAsync(HttpClient client, string prefix)
    {
        var email = $"{prefix}.{Guid.NewGuid():N}@example.com";
        const string password = "Password1";

        var registerResp = await client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
        registerResp.EnsureSuccessStatusCode();
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        loginResp.EnsureSuccessStatusCode();

        var payload = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        return payload.GetProperty("accessToken").GetString()
            ?? throw new InvalidOperationException("Login response did not include accessToken.");
    }
}
