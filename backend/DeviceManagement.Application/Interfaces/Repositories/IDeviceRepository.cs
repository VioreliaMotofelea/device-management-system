using DeviceManagement.Domain.Entities;

namespace DeviceManagement.Application.Interfaces.Repositories;

public interface IDeviceRepository
{
    Task<List<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(int id);
    Task<Device?> GetByNameAndManufacturerAsync(string name, string manufacturer);
    Task AddAsync(Device device);
    Task UpdateAsync(Device device);
    Task DeleteAsync(Device device);
    Task<bool> ExistsAsync(string name, string manufacturer);
    Task SaveChangesAsync();
}