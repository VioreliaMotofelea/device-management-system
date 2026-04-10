using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DeviceManagement.Tests.Integration.Fixtures;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class UsersCrudHappyPathTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UsersCrudHappyPathTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UsersCrud_CreateUpdateDelete_Works()
    {
        using var client = _factory.CreateClient();
        var token = await TestAuthHelper.RegisterAndLoginAsync(client, "users-happy-admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var email = $"users.happy.{Guid.NewGuid():N}@example.com";
        var createResp = await client.PostAsJsonAsync("/api/users", new
        {
            Email = email,
            Password = "Password1",
            Name = "Happy User",
            Role = "Employee",
            Location = "London"
        });
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var created = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt32();

        var updateResp = await client.PutAsJsonAsync($"/api/users/{id}", new
        {
            Email = email,
            Name = "Happy Updated",
            Role = "Manager",
            Location = "Berlin"
        });
        updateResp.EnsureSuccessStatusCode();
        var updated = await updateResp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Happy Updated", updated.GetProperty("name").GetString());

        var deleteResp = await client.DeleteAsync($"/api/users/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
    }

}
