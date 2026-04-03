namespace DeviceManagement.Domain.Constants;

public static class DeviceTypes
{
    public const string Phone = "phone";
    public const string Tablet = "tablet";

    private static readonly HashSet<string> Allowed =
        new(StringComparer.OrdinalIgnoreCase) { Phone, Tablet };

    public static bool IsAllowed(string value) => Allowed.Contains(value);
}
