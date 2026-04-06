using DeviceManagement.Application.DTOs.Devices;

namespace DeviceManagement.Application.Interfaces.Services;

public interface IDeviceDescriptionService
{
    Task<GenerateDeviceDescriptionResponseDto> GenerateAsync(
        GenerateDeviceDescriptionRequestDto dto,
        CancellationToken cancellationToken = default);
}
