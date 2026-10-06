using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IEmployeeRepository : IBaseRepository<EmployeeDTO>
{
    EmployeeDTO? GetByAccountId(int accountId);
    IEnumerable<EmployeeDTO> GetSubordinates(int managerEmployeeId);
    IEnumerable<RoleDTO> GetRoles(int employeeId);
    void AssignRole(EmployeeRoleDTO employeeRole);
    void UnassignRole(EmployeeRoleDTO employeeRole);
}