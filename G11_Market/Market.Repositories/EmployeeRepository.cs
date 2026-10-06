using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class EmployeeRepository(DbConnection connection) : BaseRepository<EmployeeDTO>(connection), IEmployeeRepository
{
    public EmployeeDTO? GetByAccountId(int accountId)
    {
        if (accountId <= 0)
            throw new ArgumentOutOfRangeException(nameof(accountId));

        return Search(e => e.AccountId == accountId).FirstOrDefault();
    }

    public IEnumerable<EmployeeDTO> GetSubordinates(int managerEmployeeId)
    {
        if (managerEmployeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(managerEmployeeId));

        return Search(e => e.ManagerEmployeeId == managerEmployeeId);
    }

    public IEnumerable<RoleDTO> GetRoles(int employeeId)
    {
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        return _connection.Query<RoleDTO>(
            "sp_GetEmployeeRolesByEmployeeId",
            new { EmployeeId = employeeId },
            commandType: CommandType.StoredProcedure);
    }

    public void AssignRole(EmployeeRoleDTO employeeRole)
    {
        ArgumentNullException.ThrowIfNull(employeeRole);
        if (employeeRole.EmployeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeRole.EmployeeId));
        if (employeeRole.RoleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeRole.RoleId));

        _connection.Execute(
            "sp_AssignEmployeeRole",
            new
            {
                employeeRole.EmployeeId,
                employeeRole.RoleId
            },
            commandType: CommandType.StoredProcedure);
    }

    public void UnassignRole(EmployeeRoleDTO employeeRole)
    {
        ArgumentNullException.ThrowIfNull(employeeRole);
        if (employeeRole.EmployeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeRole.EmployeeId));
        if (employeeRole.RoleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeRole.RoleId));

        _connection.Execute(
            "sp_UnassignEmployeeRole",
            new
            {
                EmployeeId = employeeRole.EmployeeId,
                RoleId = employeeRole.RoleId
            },
            commandType: CommandType.StoredProcedure);
    }
}