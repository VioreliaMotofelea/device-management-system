using DeviceManagement.Application.DTOs;

namespace DeviceManagement.Application.Interfaces.Services;

public interface IDeviceService
{
    Task<List<DeviceResponseDto>> GetAllAsync();
    Task<DeviceResponseDto?> GetByIdAsync(int id);
    Task<DeviceResponseDto> CreateAsync(CreateDeviceDto dto);
    Task<DeviceResponseDto?> UpdateAsync(int id, UpdateDeviceDto dto);
    Task<bool> DeleteAsync(int id);
}
