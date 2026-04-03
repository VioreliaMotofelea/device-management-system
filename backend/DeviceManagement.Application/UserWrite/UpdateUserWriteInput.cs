namespace DeviceManagement.Application.UserWrite;

public sealed record UpdateUserWriteInput(
    string Email,
    string? Password,
    string Name,
    string Role,
    string Location);
