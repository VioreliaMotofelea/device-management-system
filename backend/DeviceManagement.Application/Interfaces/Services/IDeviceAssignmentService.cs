using DeviceManagement.Application.DTOs.Devices;

namespace DeviceManagement.Application.Interfaces.Services;

public interface IDeviceAssignmentService
{
    Task<DeviceResponseDto> AssignToCurrentUserAsync(int deviceId, int userId, CancellationToken cancellationToken = default);
    Task<DeviceResponseDto> UnassignFromCurrentUserAsync(int deviceId, int userId, CancellationToken cancellationToken = default);
}
