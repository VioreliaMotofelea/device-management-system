using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Infrastructure.Services;

namespace DeviceManagement.Tests.Unit.Services;

public sealed class DeviceServiceSearchTests
{
    [Fact]
    public async Task SearchAsync_ThrowsValidation_WhenQueryIsEmpty()
    {
        var sut = new DeviceService(new FakeDeviceRepository([]));

        await Assert.ThrowsAsync<ValidationException>(() => sut.SearchAsync("   "));
    }

    [Fact]
    public async Task SearchAsync_RanksNameMatchAboveRamOnlyMatch()
    {
        var devices = new List<Device>
        {
            new()
            {
                Id = 1,
                Name = "Pixel 8",
                Manufacturer = "Google",
                Type = "phone",
                OperatingSystem = "Android",
                OsVersion = "14",
                Processor = "Tensor G3",
                RamAmount = "8 GB",
                Location = "London"
            },
            new()
            {
                Id = 2,
                Name = "Work Phone",
                Manufacturer = "Contoso",
                Type = "phone",
                OperatingSystem = "Android",
                OsVersion = "14",
                Processor = "Midrange",
                RamAmount = "8 GB",
                Location = "London"
            }
        };

        var sut = new DeviceService(new FakeDeviceRepository(devices));

        var result = await sut.SearchAsync("pixel 8");

        Assert.NotEmpty(result);
        Assert.Equal("Pixel 8", result[0].Name);
    }

    [Fact]
    public async Task SearchAsync_NormalizesPunctuationAndCase()
    {
        var devices = new List<Device>
        {
            new()
            {
                Id = 10,
                Name = "Galaxy S23",
                Manufacturer = "Samsung",
                Type = "phone",
                OperatingSystem = "Android",
                OsVersion = "14",
                Processor = "Snapdragon 8 Gen 2",
                RamAmount = "8 GB",
                Location = "Berlin"
            }
        };

        var sut = new DeviceService(new FakeDeviceRepository(devices));

        var result = await sut.SearchAsync("SNAPDRAGON,8");

        Assert.Single(result);
        Assert.Equal("Galaxy S23", result[0].Name);
    }

    [Fact]
    public async Task SearchAsync_PrioritizesExactTokenOverPartialMatch_InSameFieldWeight()
    {
        var devices = new List<Device>
        {
            new()
            {
                Id = 1,
                Name = "Snap Core",
                Manufacturer = "Contoso",
                Type = "phone",
                OperatingSystem = "Android",
                OsVersion = "14",
                Processor = "Midrange",
                RamAmount = "6 GB",
                Location = "London"
            },
            new()
            {
                Id = 2,
                Name = "Snappy Pro",
                Manufacturer = "Contoso",
                Type = "phone",
                OperatingSystem = "Android",
                OsVersion = "14",
                Processor = "Midrange",
                RamAmount = "6 GB",
                Location = "London"
            }
        };

        var sut = new DeviceService(new FakeDeviceRepository(devices));

        var result = await sut.SearchAsync("snap");

        Assert.Equal(2, result.Count);
        Assert.Equal("Snap Core", result[0].Name);
        Assert.Equal("Snappy Pro", result[1].Name);
    }

    [Fact]
    public async Task SearchAsync_UsesIdAsDeterministicTieBreaker_WhenScoresEqual()
    {
        var devices = new List<Device>
        {
            new()
            {
                Id = 20,
                Name = "Tie Alpha",
                Manufacturer = "Contoso",
                Type = "phone",
                OperatingSystem = "Android",
                OsVersion = "14",
                Processor = "Chip",
                RamAmount = "8 GB",
                Location = "London"
            },
            new()
            {
                Id = 10,
                Name = "Tie Beta",
                Manufacturer = "Contoso",
                Type = "phone",
                OperatingSystem = "Android",
                OsVersion = "14",
                Processor = "Chip",
                RamAmount = "8 GB",
                Location = "London"
            }
        };

        var sut = new DeviceService(new FakeDeviceRepository(devices));

        var result = await sut.SearchAsync("contoso");

        Assert.Equal(2, result.Count);
        Assert.Equal(10, result[0].Id);
        Assert.Equal(20, result[1].Id);
    }

    private sealed class FakeDeviceRepository : IDeviceRepository
    {
        private readonly List<Device> _devices;

        public FakeDeviceRepository(List<Device> devices)
        {
            _devices = devices;
        }

        public Task<List<Device>> GetAllAsync() => Task.FromResult(_devices);

        public Task<Device?> GetByIdAsync(int id) => Task.FromResult(_devices.FirstOrDefault(x => x.Id == id));

        public Task<Device?> GetByIdForUpdateAsync(int id) => Task.FromResult(_devices.FirstOrDefault(x => x.Id == id));

        public Task<Device?> GetByNameAndManufacturerAsync(string name, string manufacturer)
            => Task.FromResult(_devices.FirstOrDefault(x => x.Name == name && x.Manufacturer == manufacturer));

        public Task AddAsync(Device device)
        {
            _devices.Add(device);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Device device) => Task.CompletedTask;

        public Task DeleteAsync(Device device)
        {
            _devices.Remove(device);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string name, string manufacturer)
            => Task.FromResult(_devices.Any(x => x.Name == name && x.Manufacturer == manufacturer));

        public Task<bool> AnyAssignedToUserAsync(int userId)
            => Task.FromResult(_devices.Any(x => x.AssignedUserId == userId));

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
