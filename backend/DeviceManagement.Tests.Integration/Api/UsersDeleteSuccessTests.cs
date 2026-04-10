using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DeviceManagement.Tests.Integration.Fixtures;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class UsersDeleteSuccessTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UsersDeleteSuccessTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeleteUser_WithoutAssignedDevices_ReturnsNoContent()
    {
        using var client = _factory.CreateClient();
        var adminToken = await TestAuthHelper.RegisterAndLoginAsync(client, "admin-del-ok");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var createResp = await client.PostAsJsonAsync("/api/users", new
        {
            Email = $"to.delete.{Guid.NewGuid():N}@example.com",
            Password = "Password1",
            Name = "Delete Me",
            Role = "Employee",
            Location = "London"
        });
        createResp.EnsureSuccessStatusCode();

        var payload = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        var id = payload.GetProperty("id").GetInt32();

        var deleteResp = await client.DeleteAsync($"/api/users/{id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
    }
}
