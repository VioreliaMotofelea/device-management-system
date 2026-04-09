using DeviceManagement.Application.DTOs.Users;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.UserWrite;

namespace DeviceManagement.Tests.Unit.Application;

public sealed class UserInputParserTests
{
    [Fact]
    public void ParseCreate_NormalizesAndTrims()
    {
        var dto = new CreateUserDto
        {
            Email = "  USER@Example.com ",
            Password = "Password1",
            Name = "  Alice  ",
            Role = "  Employee ",
            Location = "  London "
        };

        var parsed = UserInputParser.ParseCreate(dto);

        Assert.Equal("user@example.com", parsed.Email);
        Assert.Equal("Alice", parsed.Name);
        Assert.Equal("Employee", parsed.Role);
        Assert.Equal("London", parsed.Location);
    }

    [Fact]
    public void ParseUpdate_AllowsEmptyPasswordAsNull()
    {
        var dto = new UpdateUserDto
        {
            Email = "user@example.com",
            Password = "   ",
            Name = "Alice",
            Role = "Employee",
            Location = "London"
        };

        var parsed = UserInputParser.ParseUpdate(dto);

        Assert.Null(parsed.Password);
    }

    [Fact]
    public void ParseUpdate_Throws_WhenPasswordTooShort()
    {
        var dto = new UpdateUserDto
        {
            Email = "user@example.com",
            Password = "123",
            Name = "Alice",
            Role = "Employee",
            Location = "London"
        };

        var ex = Assert.Throws<ValidationException>(() => UserInputParser.ParseUpdate(dto));
        Assert.Contains("Password must be at least", ex.Message);
    }
}
