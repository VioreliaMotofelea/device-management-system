using DeviceManagement.Application.Common;
using DeviceManagement.Application.DTOs.Auth;
using DeviceManagement.Application.Exceptions;

namespace DeviceManagement.Application.Validators.Auth;

public static class LoginRequestValidator
{
    public static (string Email, string Password) Validate(LoginRequestDto? dto)
    {
        if (dto is null)
            throw new ValidationException("Request body is required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ValidationException("Password is required.");

        var email = EmailNormalizer.Normalize(dto.Email);
        return (email, dto.Password);
    }
}
