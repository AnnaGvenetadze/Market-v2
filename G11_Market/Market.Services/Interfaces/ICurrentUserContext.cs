using Market.Services.Enums;

namespace Market.Services.Interfaces;

public interface ICurrentUserContext
{
    int EmployeeId { get; }

    bool IsAuthenticated { get; }

    bool HasRole(Role role);
}