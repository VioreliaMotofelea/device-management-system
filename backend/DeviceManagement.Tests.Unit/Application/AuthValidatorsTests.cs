using DeviceManagement.Application.DTOs.Auth;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Validators.Auth;

namespace DeviceManagement.Tests.Unit.Application;

public sealed class AuthValidatorsTests
{
    [Fact]
    public void LoginValidator_NormalizesEmail()
    {
        var (email, password) = LoginRequestValidator.Validate(new LoginRequestDto
        {
            Email = "  USER@Example.COM  ",
            Password = "Password1"
        });

        Assert.Equal("user@example.com", email);
        Assert.Equal("Password1", password);
    }

    [Fact]
    public void RegisterValidator_Throws_WhenPasswordTooShort()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            RegisterRequestValidator.Validate(new RegisterRequestDto
            {
                Email = "user@example.com",
                Password = "123"
            }));

        Assert.Contains("Password must be at least", ex.Message);
    }
}
