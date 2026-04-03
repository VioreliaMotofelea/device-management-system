namespace DeviceManagement.Application.Exceptions;

public sealed class ValidationException : AppException
{
    public ValidationException(string message) : base(message, 400)
    {
    }
}
