using System.Net;
using DeviceManagement.Tests.Integration.Fixtures;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DevicesEndpointAuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DevicesEndpointAuthTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetDevices_WithoutToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/devices");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SearchDevices_WithoutToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/devices/search?q=apple");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
