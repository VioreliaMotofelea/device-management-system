using System.Net.Http.Headers;
using System.Net.Http.Json;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceManagement.Tests.Integration.Infrastructure;

public sealed class DeviceRepositoryIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DeviceRepositoryIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetByNameAndManufacturerAsync_ReturnsDevice_WhenExists()
    {
        using var client = _factory.CreateClient();
        var token = await TestAuthHelper.RegisterAndLoginAsync(client, "repo-hit");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var name = $"repo-{Guid.NewGuid():N}".Substring(0, 12);
        const string manufacturer = "RepoBrand";

        var create = await client.PostAsJsonAsync(
            "/api/devices",
            TestPayloads.BuildDevicePayload(
                name,
                manufacturer,
                description: "Repository integration test device."));
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

}
