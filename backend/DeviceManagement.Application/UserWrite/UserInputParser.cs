using DeviceManagement.Application.Common;
using DeviceManagement.Application.DTOs.Users;
using DeviceManagement.Application.Exceptions;
using DeviceManagement.Application.Validation;

namespace DeviceManagement.Application.UserWrite;

public static class UserInputParser
{
    public static CreateUserWriteInput ParseCreate(CreateUserDto dto)
    {
        if (dto is null)
            throw new ValidationException("Request body is required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ValidationException("Password is required.");
        if (dto.Password.Length < ValidationConstants.MinPasswordLength)
            throw new ValidationException($"Password must be at least {ValidationConstants.MinPasswordLength} characters.");
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Name is required.");
        if (string.IsNullOrWhiteSpace(dto.Role))
            throw new ValidationException("Role is required.");
        if (string.IsNullOrWhiteSpace(dto.Location))
            throw new ValidationException("Location is required.");

        var email = EmailNormalizer.Normalize(dto.Email);

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

        var email = EmailNormalizer.Normalize(dto.Email);

        string? password = string.IsNullOrWhiteSpace(dto.Password) ? null : dto.Password;
        if (password is not null && password.Length < ValidationConstants.MinPasswordLength)
            throw new ValidationException($"Password must be at least {ValidationConstants.MinPasswordLength} characters.");

        return new UpdateUserWriteInput(
            email,
            password,
            dto.Name.Trim(),
            dto.Role.Trim(),
            dto.Location.Trim());
    }
}
