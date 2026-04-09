using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DeviceManagement.Tests.Integration.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DevicesCrudEdgeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DevicesCrudEdgeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateDevice_DuplicateNameManufacturer_ReturnsConflict()
    {
        using var client = _factory.CreateClient();
        var token = await TestAuthHelper.RegisterAndLoginAsync(client, "dev-dup");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var unique = $"dup-{Guid.NewGuid():N}".Substring(0, 12);
        var body = TestPayloads.BuildDevicePayload(unique, "BrandX", description: "Device CRUD edge test payload.");

        var first = await client.PostAsJsonAsync("/api/devices", body);
        first.EnsureSuccessStatusCode();

        var second = await client.PostAsJsonAsync("/api/devices", body);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task UpdateDevice_NotFound_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var token = await TestAuthHelper.RegisterAndLoginAsync(client, "dev-upd-miss");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsJsonAsync(
            "/api/devices/999999",
            TestPayloads.BuildDevicePayload("missing", "BrandY", description: "Device CRUD edge test payload."));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDevice_NotFound_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var token = await TestAuthHelper.RegisterAndLoginAsync(client, "dev-del-miss");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync("/api/devices/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

}
