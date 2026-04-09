using DeviceManagement.Application.DTOs.Devices;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Infrastructure.Services;

namespace DeviceManagement.Tests.Unit.Services;

public sealed class DeviceServiceCrudTests
{
    [Fact]
    public async Task CreateAsync_ThrowsConflict_WhenDuplicateExists()
    {
        var repo = new FakeRepo { Exists = true };
        var sut = new DeviceService(repo);

        await Assert.ThrowsAsync<ConflictException>(() => sut.CreateAsync(BuildCreateDto("Pixel 8", "Google")));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenDeviceMissing()
    {
        var repo = new FakeRepo { DeviceById = null };
        var sut = new DeviceService(repo);

        await Assert.ThrowsAsync<NotFoundException>(() => sut.UpdateAsync(7, BuildUpdateDto("Pixel 8", "Google")));
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFound_WhenDeviceMissing()
    {
        var repo = new FakeRepo { DeviceById = null };
        var sut = new DeviceService(repo);

        await Assert.ThrowsAsync<NotFoundException>(() => sut.DeleteAsync(7));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsConflict_WhenDuplicateBelongsToAnotherDevice()
    {
        var repo = new FakeRepo
        {
            DeviceById = BuildDevice(7, "Current", "Vendor"),
            DuplicateByNameManufacturer = BuildDevice(99, "Pixel 8", "Google")
        };
        var sut = new DeviceService(repo);

        await Assert.ThrowsAsync<ConflictException>(() => sut.UpdateAsync(7, BuildUpdateDto("Pixel 8", "Google")));
    }

    private static CreateDeviceDto BuildCreateDto(string name, string manufacturer) => new()
    {
        Name = name,
        Manufacturer = manufacturer,
        Type = "phone",
        OperatingSystem = "Android",
        OsVersion = "14",
        Processor = "Tensor G3",
        RamAmount = "8 GB",
        Description = "Business device",
        Location = "London"
    };

    private static UpdateDeviceDto BuildUpdateDto(string name, string manufacturer) => new()
    {
        Name = name,
        Manufacturer = manufacturer,
        Type = "phone",
        OperatingSystem = "Android",
        OsVersion = "14",
        Processor = "Tensor G3",
        RamAmount = "8 GB",
        Description = "Updated",
        Location = "London"
    };

    private static Device BuildDevice(int id, string name, string manufacturer) => new()
    {
        Id = id,
        Name = name,
        Manufacturer = manufacturer,
        Type = "phone",
        OperatingSystem = "Android",
        OsVersion = "14",
        Processor = "Tensor G3",
        RamAmount = "8 GB",
        Description = "Device",
        Location = "London"
    };

    private sealed class FakeRepo : IDeviceRepository
    {
        public bool Exists { get; set; }
        public Device? DeviceById { get; set; } = BuildDevice(1, "Seed", "SeedCorp");
        public Device? DuplicateByNameManufacturer { get; set; }

        public Task<List<Device>> GetAllAsync() => Task.FromResult(new List<Device>());
        public Task<Device?> GetByIdAsync(int id) => Task.FromResult(DeviceById);
        public Task<Device?> GetByIdForUpdateAsync(int id) => Task.FromResult(DeviceById);
        public Task<Device?> GetByNameAndManufacturerAsync(string name, string manufacturer) => Task.FromResult(DuplicateByNameManufacturer);
        public Task AddAsync(Device device) => Task.CompletedTask;
        public Task UpdateAsync(Device device) => Task.CompletedTask;
        public Task DeleteAsync(Device device) => Task.CompletedTask;
        public Task<bool> ExistsAsync(string name, string manufacturer) => Task.FromResult(Exists);
        public Task<bool> AnyAssignedToUserAsync(int userId) => Task.FromResult(false);
        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
