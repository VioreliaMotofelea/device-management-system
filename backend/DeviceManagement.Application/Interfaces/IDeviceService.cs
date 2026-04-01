using DeviceManagement.Application.DTOs;

namespace DeviceManagement.Application.Interfaces;

public interface IDeviceService
{
    Task<List<DeviceDto>> GetAllAsync();
    Task<DeviceDto?> GetByIdAsync(int id);
    Task<DeviceDto> CreateAsync(CreateDeviceDto dto);
    Task<bool> UpdateAsync(int id, UpdateDeviceDto dto);
    Task<bool> DeleteAsync(int id);
}
