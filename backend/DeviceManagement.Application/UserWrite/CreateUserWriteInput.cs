namespace DeviceManagement.Application.UserWrite;

public sealed record CreateUserWriteInput(
    string Email,
    string Password,
    string Name,
    string Role,
    string Location);
