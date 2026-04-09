using DeviceManagement.Application.DTOs.Devices;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Validators.Devices;

namespace DeviceManagement.Tests.Unit.Application;

public sealed class GenerateDeviceDescriptionRequestValidatorTests
{
    [Fact]
    public void Validate_Throws_WhenTypeInvalid()
    {
        var dto = BuildValid();
        dto.Type = "laptop";

        var ex = Assert.Throws<ValidationException>(() => GenerateDeviceDescriptionRequestValidator.Validate(dto));

        Assert.Contains("Type must be", ex.Message);
    }

    [Fact]
    public void Validate_Passes_ForValidDto()
    {
        var dto = BuildValid();

        var action = () => GenerateDeviceDescriptionRequestValidator.Validate(dto);

        var ex = Record.Exception(action);
        Assert.Null(ex);
    }

    private static GenerateDeviceDescriptionRequestDto BuildValid() => new()
    {
        Name = "Pixel 8",
        Manufacturer = "Google",
        Type = "phone",
        OperatingSystem = "Android",
        OsVersion = "14",
        Processor = "Tensor G3",
        RamAmount = "8 GB"
    };
}
