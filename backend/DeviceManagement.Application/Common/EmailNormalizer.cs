using DeviceManagement.Application.Exceptions;

namespace DeviceManagement.Application.Common;

public static class EmailNormalizer
{
    public static string Normalize(string email)
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
