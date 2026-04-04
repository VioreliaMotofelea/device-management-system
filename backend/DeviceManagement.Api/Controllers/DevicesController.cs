using DeviceManagement.Api.Extensions;
using DeviceManagement.Application.DTOs.Devices;
using DeviceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.Api.Controllers;

[ApiController]
[Route("api/devices")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly IDeviceAssignmentService _assignmentService;

    public DevicesController(IDeviceService deviceService, IDeviceAssignmentService assignmentService)
    {
        _deviceService = deviceService;
        _assignmentService = assignmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var devices = await _deviceService.GetAllAsync();
        return Ok(devices);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var device = await _deviceService.GetByIdAsync(id);
        return Ok(device);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeviceDto dto)
    {
        var created = await _deviceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDeviceDto dto)
    {
        var updated = await _deviceService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _deviceService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:int}/assign")]
    public async Task<IActionResult> Assign(int id)
    {
        var userId = User.GetRequiredUserId();
        var device = await _assignmentService.AssignToCurrentUserAsync(id, userId);
        return Ok(device);
    }

    [HttpPost("{id:int}/unassign")]
    public async Task<IActionResult> Unassign(int id)
    {
        var userId = User.GetRequiredUserId();
        var device = await _assignmentService.UnassignFromCurrentUserAsync(id, userId);
        return Ok(device);
    }
}
