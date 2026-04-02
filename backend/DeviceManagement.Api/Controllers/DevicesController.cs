using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.Api.Controllers;

[ApiController]
[Route("api/devices")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
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
        if (device == null)
            return NotFound(new { message = $"Device with id {id} was not found." });

        return Ok(device);
    }

    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateDeviceDto dto) =>
        WriteAsync(async () =>
        {
            var created = await _deviceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        });

    [HttpPut("{id:int}")]
    public Task<IActionResult> Update(int id, [FromBody] UpdateDeviceDto dto) =>
        WriteAsync(async () =>
        {
            var updated = await _deviceService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Device with id {id} was not found." });

            return Ok(updated);
        });

    private async Task<IActionResult> WriteAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _deviceService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Device with id {id} was not found." });

        return NoContent();
    }
}
