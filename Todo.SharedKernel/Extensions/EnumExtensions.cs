using Todo.SharedKernel.Enums;

namespace Todo.SharedKernel.Extensions;

public static class EnumExtensions
{
    public static string GetRoleName(this Role role)
    {
        return role switch
        {
            Role.Admin => "Admin",
            Role.Standard => "Standard",
            Role.Premium => "Premium",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }

    public static Role GetRoleNameFromString(this string role)
    {
        return role switch
        {
            "Admin" => Role.Admin,
            "Standard" => Role.Standard,
            "Premium" => Role.Premium,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }

    public static string GetErrorLevel(this ErrorLevel level)
    {
        return level switch
        {
            ErrorLevel.Info => "Information",
            ErrorLevel.Low => "Low",
            ErrorLevel.Medium => "Medium",
            ErrorLevel.High => "High",
            ErrorLevel.Critical => "Critical",
            ErrorLevel.Fatal => "Fatal",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }
}