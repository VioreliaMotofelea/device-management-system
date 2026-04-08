using DeviceManagement.Application.DTOs.Devices;

namespace DeviceManagement.Application.Interfaces.Services;

public interface IDeviceService
{
    Task<List<DeviceResponseDto>> GetAllAsync();
    Task<List<DeviceResponseDto>> SearchAsync(string query);
    Task<DeviceResponseDto> GetByIdAsync(int id);
    Task<DeviceResponseDto> CreateAsync(CreateDeviceDto dto);
    Task<DeviceResponseDto> UpdateAsync(int id, UpdateDeviceDto dto);
    Task DeleteAsync(int id);
}
