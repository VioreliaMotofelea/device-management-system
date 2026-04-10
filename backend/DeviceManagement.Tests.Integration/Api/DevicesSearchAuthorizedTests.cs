using System.Net;
using System.Net.Http.Headers;
using DeviceManagement.Tests.Integration.Fixtures;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DevicesSearchAuthorizedTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DevicesSearchAuthorizedTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SearchDevices_WithValidToken_ReturnsOk()
    {
        using var client = _factory.CreateClient();

        var token = await TestAuthHelper.RegisterAndLoginAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/devices/search?q=apple");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.StartsWith("[", body.TrimStart());
    }

    [Fact]
    public async Task SearchDevices_WithEmptyQuery_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var token = await TestAuthHelper.RegisterAndLoginAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/devices/search?q=   ");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

}
