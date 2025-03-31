using Todo.Shared.Enums;

namespace Todo.User.Domain.Extensions;

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
}