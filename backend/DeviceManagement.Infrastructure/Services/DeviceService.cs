using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Interfaces;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Services;

public class DeviceService : IDeviceService
{
    private readonly AppDbContext _context;

    public DeviceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DeviceDto>> GetAllAsync()
    {
        return await _context.Devices
            .Select(d => new DeviceDto
            {
                Id = d.Id,
                Name = d.Name,
                Manufacturer = d.Manufacturer,
                Type = d.Type,
                OperatingSystem = d.OperatingSystem,
                OsVersion = d.OsVersion,
                Processor = d.Processor,
                RamAmount = d.RamAmount,
                Description = d.Description,
                Location = d.Location
            })
            .ToListAsync();
    }

    public async Task<DeviceDto?> GetByIdAsync(int id)
    {
        var d = await _context.Devices.FindAsync(id);
        if (d == null) return null;

        return new DeviceDto
        {
            Id = d.Id,
            Name = d.Name,
            Manufacturer = d.Manufacturer,
            Type = d.Type,
            OperatingSystem = d.OperatingSystem,
            OsVersion = d.OsVersion,
            Processor = d.Processor,
            RamAmount = d.RamAmount,
            Description = d.Description,
            Location = d.Location
        };
    }

    public async Task<DeviceDto> CreateAsync(CreateDeviceDto dto)
    {
        var device = new Device
        {
            Name = dto.Name,
            Manufacturer = dto.Manufacturer,
            Type = dto.Type,
            OperatingSystem = dto.OperatingSystem,
            OsVersion = dto.OsVersion,
            Processor = dto.Processor,
            RamAmount = dto.RamAmount,
            Location = dto.Location
        };

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(device.Id)!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateDeviceDto dto)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null) return false;

        device.Name = dto.Name;
        device.Manufacturer = dto.Manufacturer;
        device.Type = dto.Type;
        device.OperatingSystem = dto.OperatingSystem;
        device.OsVersion = dto.OsVersion;
        device.Processor = dto.Processor;
        device.RamAmount = dto.RamAmount;
        device.Location = dto.Location;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null) return false;

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();
        return true;
    }
}
