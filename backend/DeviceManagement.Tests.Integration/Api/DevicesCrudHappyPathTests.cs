using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DeviceManagement.Tests.Integration.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DevicesCrudHappyPathTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DevicesCrudHappyPathTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeviceCrud_CreateGetUpdate_Works()
    {
        using var client = _factory.CreateClient();
        var token = await TestAuthHelper.RegisterAndLoginAsync(client, "dev-happy");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var name = $"happy-{Guid.NewGuid():N}".Substring(0, 12);
        var createResp = await client.PostAsJsonAsync("/api/devices", TestPayloads.BuildDevicePayload(name, "BrandHappy", "8 GB", description: "Happy CRUD path device."));
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

        var created = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt32();

        var getResp = await client.GetAsync($"/api/devices/{id}");
        getResp.EnsureSuccessStatusCode();
        var fetched = await getResp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(name, fetched.GetProperty("name").GetString());

        var updateResp = await client.PutAsJsonAsync($"/api/devices/{id}", TestPayloads.BuildDevicePayload(name, "BrandHappy", "12 GB", description: "Happy CRUD path device."));
        updateResp.EnsureSuccessStatusCode();
        var updated = await updateResp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("12 GB", updated.GetProperty("ramAmount").GetString());
    }

}
