using DeviceManagement.Application.DTOs.Auth;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Validators.Auth;

namespace DeviceManagement.Tests.Unit.Application;

public sealed class AuthValidatorsTests
{
    [Fact]
    public void LoginValidator_Throws_WhenBodyNull()
    {
        Assert.Throws<ValidationException>(() => LoginRequestValidator.Validate(null));
    }

    [Fact]
    public void RegisterValidator_Throws_WhenEmailMissing()
    {
        Assert.Throws<ValidationException>(() => RegisterRequestValidator.Validate(new RegisterRequestDto
        {
            Email = " ",
            Password = "Password1",
            ConfirmPassword = "Password1"
        }));
    }

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
                Password = "123",
                ConfirmPassword = "123"
            }));

        Assert.Contains("Password must be at least", ex.Message);
    }

    [Fact]
    public void RegisterValidator_Throws_WhenConfirmPasswordMissing()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            RegisterRequestValidator.Validate(new RegisterRequestDto
            {
                Email = "user@example.com",
                Password = "Password1",
                ConfirmPassword = " "
            }));

        Assert.Equal("Confirm password is required.", ex.Message);
    }

    [Fact]
    public void RegisterValidator_Throws_WhenPasswordAndConfirmDoNotMatch()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            RegisterRequestValidator.Validate(new RegisterRequestDto
            {
                Email = "user@example.com",
                Password = "Password1",
                ConfirmPassword = "Password2"
            }));

        Assert.Equal("Password and confirm password must match.", ex.Message);
    }
}
