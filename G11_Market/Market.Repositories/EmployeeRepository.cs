using System.Data;
using System.Data.Common;
using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class EmployeeRepository(DbConnection connection) : BaseRepository<EmployeeDTO>(connection), IEmployeeRepository
{
    public EmployeeDTO? GetByEmployeeCode(string employeeCode) => Search(e => e.EmployeeCode == employeeCode).FirstOrDefault();

    public EmployeeDTO? GetByAccountId(int accountId) => Search(e => e.AccountId == accountId).FirstOrDefault();

    public IEnumerable<EmployeeDTO> GetSubordinates(int managerEmployeeId) => Search(e => e.ManagerEmployeeId == managerEmployeeId);

    public IEnumerable<EmployeeDTO> GetAllActive() => Search(e => e.IsDeleted == false);

    public IEnumerable<RoleDTO> GetRoles(int employeeId)
    {
        return _connection.Query<RoleDTO>(
            "sp_GetEmployeeRolesByEmployeeId",
            new { EmployeeId = employeeId },
            commandType: CommandType.StoredProcedure);
    }

    public void AssignRole(EmployeeRoleDTO employeeRole)
    {
        _connection.Execute(
            "sp_AssignEmployeeRole",
            new
            {
                EmployeeId = employeeRole.EmployeeId,
                RoleId = employeeRole.RoleId
            },
            commandType: CommandType.StoredProcedure);
    }

    public void UnassignRole(EmployeeRoleDTO employeeRole)
    {
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