using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Exceptions;

namespace DeviceManagement.Application.UserWrite;

public static class UserInputParser
{
    private const int MinPasswordLength = 6;

    public static CreateUserWriteInput ParseCreate(CreateUserDto dto)
    {
        if (dto is null)
            throw new ValidationException("Request body is required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ValidationException("Password is required.");
        if (dto.Password.Length < MinPasswordLength)
            throw new ValidationException($"Password must be at least {MinPasswordLength} characters.");
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Name is required.");
        if (string.IsNullOrWhiteSpace(dto.Role))
            throw new ValidationException("Role is required.");
        if (string.IsNullOrWhiteSpace(dto.Location))
            throw new ValidationException("Location is required.");

        var email = NormalizeEmail(dto.Email);

        return new CreateUserWriteInput(
            email,
            dto.Password,
            dto.Name.Trim(),
            dto.Role.Trim(),
            dto.Location.Trim());
    }

    public static UpdateUserWriteInput ParseUpdate(UpdateUserDto dto)
    {
        if (dto is null)
            throw new ValidationException("Request body is required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Name is required.");
        if (string.IsNullOrWhiteSpace(dto.Role))
            throw new ValidationException("Role is required.");
        if (string.IsNullOrWhiteSpace(dto.Location))
            throw new ValidationException("Location is required.");

        var email = NormalizeEmail(dto.Email);

        string? password = string.IsNullOrWhiteSpace(dto.Password) ? null : dto.Password;
        if (password is not null && password.Length < MinPasswordLength)
            throw new ValidationException($"Password must be at least {MinPasswordLength} characters.");

        return new UpdateUserWriteInput(
            email,
            password,
            dto.Name.Trim(),
            dto.Role.Trim(),
            dto.Location.Trim());
    }

    private static string NormalizeEmail(string email)
    {
        var trimmed = email.Trim();
        if (trimmed.Length == 0)
            throw new ValidationException("Email is required.");

        var at = trimmed.IndexOf('@');
        if (at <= 0 || at == trimmed.Length - 1 || trimmed.IndexOf('@', at + 1) >= 0)
            throw new ValidationException("Email format is invalid.");

        var domain = trimmed[(at + 1)..];
        if (!domain.Contains('.'))
            throw new ValidationException("Email format is invalid.");

        return trimmed.ToLowerInvariant();
    }
}
