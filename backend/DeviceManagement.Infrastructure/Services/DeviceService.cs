using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Application.Interfaces.Services;
using DeviceManagement.Domain.Entities;

namespace DeviceManagement.Infrastructure.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<List<DeviceResponseDto>> GetAllAsync()
    {
        var devices = await _deviceRepository.GetAllAsync();

        return devices.Select(MapToResponse).ToList();
    }

    public async Task<DeviceResponseDto?> GetByIdAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);

        return device == null ? null : MapToResponse(device);
    }

    public async Task<DeviceResponseDto> CreateAsync(CreateDeviceDto dto)
    {
        ValidateCreateOrUpdate(dto);

        var exists = await _deviceRepository.ExistsAsync(dto.Name.Trim(), dto.Manufacturer.Trim());
        if (exists)
            throw new InvalidOperationException("A device with the same name and manufacturer already exists.");

        var device = new Device
        {
            Name = dto.Name.Trim(),
            Manufacturer = dto.Manufacturer.Trim(),
            Type = dto.Type.Trim(),
            OperatingSystem = dto.OperatingSystem.Trim(),
            OsVersion = dto.OsVersion.Trim(),
            Processor = dto.Processor.Trim(),
            RamAmount = dto.RamAmount.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            Location = dto.Location.Trim()
        };

        await _deviceRepository.AddAsync(device);
        await _deviceRepository.SaveChangesAsync();

        return MapToResponse(device);
    }

    public async Task<DeviceResponseDto?> UpdateAsync(int id, UpdateDeviceDto dto)
    {
        ValidateCreateOrUpdate(dto);

        var device = await _deviceRepository.GetByIdAsync(id);
        if (device == null)
            return null;

        var duplicate = await _deviceRepository.GetByNameAndManufacturerAsync(dto.Name.Trim(), dto.Manufacturer.Trim());
        if (duplicate != null && duplicate.Id != id)
            throw new InvalidOperationException("Another device with the same name and manufacturer already exists.");

        device.Name = dto.Name.Trim();
        device.Manufacturer = dto.Manufacturer.Trim();
        device.Type = dto.Type.Trim();
        device.OperatingSystem = dto.OperatingSystem.Trim();
        device.OsVersion = dto.OsVersion.Trim();
        device.Processor = dto.Processor.Trim();
        device.RamAmount = dto.RamAmount.Trim();
        device.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        device.Location = dto.Location.Trim();

        await _deviceRepository.UpdateAsync(device);
        await _deviceRepository.SaveChangesAsync();

        return MapToResponse(device);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        if (device == null)
            return false;

        await _deviceRepository.DeleteAsync(device);
        await _deviceRepository.SaveChangesAsync();

        return true;
    }

    private static void ValidateCreateOrUpdate(CreateDeviceDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(dto.Manufacturer)) throw new ArgumentException("Manufacturer is required.");
        if (string.IsNullOrWhiteSpace(dto.Type)) throw new ArgumentException("Type is required.");
        if (string.IsNullOrWhiteSpace(dto.OperatingSystem)) throw new ArgumentException("Operating system is required.");
        if (string.IsNullOrWhiteSpace(dto.OsVersion)) throw new ArgumentException("OS version is required.");
        if (string.IsNullOrWhiteSpace(dto.Processor)) throw new ArgumentException("Processor is required.");
        if (string.IsNullOrWhiteSpace(dto.RamAmount)) throw new ArgumentException("RAM amount is required.");
        if (string.IsNullOrWhiteSpace(dto.Location)) throw new ArgumentException("Location is required.");

        var normalizedType = dto.Type.Trim().ToLowerInvariant();
        if (normalizedType != "phone" && normalizedType != "tablet")
            throw new ArgumentException("Type must be either 'phone' or 'tablet'.");
    }

    private static DeviceResponseDto MapToResponse(Device device)
    {
        return new DeviceResponseDto
        {
            Id = device.Id,
            Name = device.Name,
            Manufacturer = device.Manufacturer,
            Type = device.Type,
            OperatingSystem = device.OperatingSystem,
            OsVersion = device.OsVersion,
            Processor = device.Processor,
            RamAmount = device.RamAmount,
            Description = device.Description,
            Location = device.Location
        };
    }
}