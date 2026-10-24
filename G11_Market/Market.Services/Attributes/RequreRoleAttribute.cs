using Market.Services.Enums;

namespace Market.Services.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireRoleAttribute : Attribute
{
    public Role[] Roles { get; }

    public RequireRoleAttribute(params Role[] roles)
    {
        if (roles == null || roles.Length == 0)
            throw new ArgumentException("At least one role is required.", nameof(roles));

        Roles = roles;
    }
}