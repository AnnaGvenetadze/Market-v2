using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IEmployeeRepository : IBaseRepository<EmployeeDTO>
{
    EmployeeDTO? GetByEmployeeCode(string employeeCode);
    EmployeeDTO? GetByAccountId(int accountId);
    IEnumerable<EmployeeDTO> GetSubordinates(int managerEmployeeId);
    IEnumerable<EmployeeDTO> GetAllActive();
    IEnumerable<RoleDTO> GetRoles(int employeeId);
    void AssignRole(EmployeeRoleDTO employeeRole);
    void UnassignRole(EmployeeRoleDTO employeeRole);
}
