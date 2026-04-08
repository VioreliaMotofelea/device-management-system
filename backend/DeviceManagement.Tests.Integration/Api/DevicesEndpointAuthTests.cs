using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DevicesEndpointAuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DevicesEndpointAuthTests(WebApplicationFactory<Program> factory)
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
}
