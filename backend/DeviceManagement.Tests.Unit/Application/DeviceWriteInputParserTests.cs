using DeviceManagement.Application.DeviceWrite;
using DeviceManagement.Application.DTOs.Devices;
using DeviceManagement.Application.Exceptions;

namespace DeviceManagement.Tests.Unit.Application;

public sealed class DeviceWriteInputParserTests
{
    [Fact]
    public void Parse_ThrowsValidation_WhenTypeInvalid()
    {
        var dto = BuildValidDto();
        dto.Type = "laptop";

        var ex = Assert.Throws<ValidationException>(() => DeviceWriteInputParser.Parse(dto));

        Assert.Contains("Type must be", ex.Message);
    }

    [Fact]
    public void Parse_TrimsAndNormalizesFields()
    {
        var dto = new CreateDeviceDto
        {
            Name = "  Pixel 8  ",
            Manufacturer = "  Google ",
            Type = "  PHONE ",
            OperatingSystem = " Android ",
            OsVersion = " 14 ",
            Processor = " Tensor G3 ",
            RamAmount = " 8 GB ",
            Description = " Great phone ",
            Location = " London "
        };

        var parsed = DeviceWriteInputParser.Parse(dto);

        Assert.Equal("Pixel 8", parsed.Name);
        Assert.Equal("Google", parsed.Manufacturer);
        Assert.Equal("phone", parsed.Type);
        Assert.Equal("8 GB", parsed.RamAmount);
    }

    private static CreateDeviceDto BuildValidDto() => new()
    {
        Name = "Pixel 8",
        Manufacturer = "Google",
        Type = "phone",
        OperatingSystem = "Android",
        OsVersion = "14",
        Processor = "Tensor G3",
        RamAmount = "8 GB",
        Description = "Business phone",
        Location = "London"
    };
}
