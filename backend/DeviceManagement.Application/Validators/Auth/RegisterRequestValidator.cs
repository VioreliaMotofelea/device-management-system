using DeviceManagement.Application.Common;
using DeviceManagement.Application.DTOs.Auth;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Validation;

namespace DeviceManagement.Application.Validators.Auth;

public static class RegisterRequestValidator
{
    public static (string Email, string Password) Validate(RegisterRequestDto? dto)
    {
        if (dto is null)
            throw new ValidationException("Request body is required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ValidationException("Password is required.");
        if (dto.Password.Length < ValidationConstants.MinPasswordLength)
            throw new ValidationException($"Password must be at least {ValidationConstants.MinPasswordLength} characters.");

        var email = EmailNormalizer.Normalize(dto.Email);
        return (email, dto.Password);
    }
}
