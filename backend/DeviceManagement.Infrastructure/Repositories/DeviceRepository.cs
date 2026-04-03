using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly AppDbContext _context;

    public DeviceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Device>> GetAllAsync()
    {
        return await _context.Devices
            .AsNoTracking()
            .Include(d => d.AssignedUser)
            .ToListAsync();
    }

    public async Task<Device?> GetByIdAsync(int id)
    {
        return await _context.Devices
            .AsNoTracking()
            .Include(d => d.AssignedUser)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Device?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Devices
            .Include(d => d.AssignedUser)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Device?> GetByNameAndManufacturerAsync(string name, string manufacturer)
    {
        return await _context.Devices.AsNoTracking().FirstOrDefaultAsync(d =>
            d.Name == name && d.Manufacturer == manufacturer);
    }

    public async Task AddAsync(Device device)
    {
        await _context.Devices.AddAsync(device);
    }

    public Task UpdateAsync(Device device)
    {
        _context.Devices.Update(device);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Device device)
    {
        _context.Devices.Remove(device);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string name, string manufacturer)
    {
        return await _context.Devices.AsNoTracking().AnyAsync(d =>
            d.Name == name && d.Manufacturer == manufacturer);
    }

    public async Task<bool> AnyAssignedToUserAsync(int userId)
    {
        return await _context.Devices.AsNoTracking()
            .AnyAsync(d => d.AssignedUserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
