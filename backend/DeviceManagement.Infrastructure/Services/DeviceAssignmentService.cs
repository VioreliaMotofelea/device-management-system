using DeviceManagement.Application.DTOs.Devices;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Application.Interfaces.Services;
using DeviceManagement.Application.Mapping;

namespace DeviceManagement.Infrastructure.Services;

public sealed class DeviceAssignmentService : IDeviceAssignmentService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceAssignmentService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<DeviceResponseDto> AssignToCurrentUserAsync(
        int deviceId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var device = await _deviceRepository.GetByIdForUpdateAsync(deviceId);
        if (device == null)
            throw new NotFoundException($"Device with id {deviceId} was not found.");

        if (device.AssignedUserId.HasValue && device.AssignedUserId.Value != userId)
            throw new ConflictException("This device is already assigned to another user.");

        device.AssignedUserId = userId;
        await _deviceRepository.UpdateAsync(device);
        await _deviceRepository.SaveChangesAsync();

        var updated = await _deviceRepository.GetByIdAsync(deviceId);
        return DeviceResponseMapper.ToDto(updated!);
    }

    public async Task<DeviceResponseDto> UnassignFromCurrentUserAsync(
        int deviceId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var device = await _deviceRepository.GetByIdForUpdateAsync(deviceId);
        if (device == null)
            throw new NotFoundException($"Device with id {deviceId} was not found.");

        if (!device.AssignedUserId.HasValue)
            throw new ValidationException("This device is not assigned.");

        if (device.AssignedUserId.Value != userId)
            throw new ForbiddenException("You can only unassign devices that are assigned to you.");

        device.AssignedUserId = null;
        device.AssignedUser = null;
        await _deviceRepository.UpdateAsync(device);
        await _deviceRepository.SaveChangesAsync();

        var updated = await _deviceRepository.GetByIdAsync(deviceId);
        return DeviceResponseMapper.ToDto(updated!);
    }
}
