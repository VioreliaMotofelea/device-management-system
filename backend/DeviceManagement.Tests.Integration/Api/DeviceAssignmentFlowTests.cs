using System.Net;
using System.Net.Http.Headers;
using DeviceManagement.Tests.Integration.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeviceManagement.Tests.Integration.Api;

public sealed class DeviceAssignmentFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeviceAssignmentFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Unassign_ByDifferentUser_ReturnsForbidden()
    {
        using var client = _factory.CreateClient();

        var userA = await TestAuthHelper.RegisterAndLoginWithUserAsync(client, "assign-a");
        var userB = await TestAuthHelper.RegisterAndLoginWithUserAsync(client, "assign-b");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userA.Token);
        var deviceId = await TestPayloads.CreateDeviceAsync(client, $"assign-{Guid.NewGuid():N}".Substring(0, 12));
        var assignResp = await client.PostAsync($"/api/devices/{deviceId}/assign", null);
        assignResp.EnsureSuccessStatusCode();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userB.Token);
        var unassignResp = await client.PostAsync($"/api/devices/{deviceId}/unassign", null);

        Assert.Equal(HttpStatusCode.Forbidden, unassignResp.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_WithAssignedDevice_ReturnsConflict()
    {
        using var client = _factory.CreateClient();

        var user = await TestAuthHelper.RegisterAndLoginWithUserAsync(client, "delete-assigned");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

        var deviceId = await TestPayloads.CreateDeviceAsync(client, $"del-{Guid.NewGuid():N}".Substring(0, 12));
        var assignResp = await client.PostAsync($"/api/devices/{deviceId}/assign", null);
        assignResp.EnsureSuccessStatusCode();

        var deleteResp = await client.DeleteAsync($"/api/users/{user.UserId}");

        Assert.Equal(HttpStatusCode.Conflict, deleteResp.StatusCode);
    }

}
