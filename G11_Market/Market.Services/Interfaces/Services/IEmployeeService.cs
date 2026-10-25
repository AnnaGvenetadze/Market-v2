using Market.DTO;
using Market.Services.Attributes;
using Market.Services.Enums;

namespace Market.Services.Interfaces.Services;

public interface IEmployeeService
{
    EmployeeDTO? GetById(int employeeId);
    IEnumerable<EmployeeDTO> GetAll();

    [RequireRole(Role.Administrator)]
    EmployeeDTO CreateEmployee(CreateEmployeeDTO dto);

    void UpdateEmployee(EmployeeDTO dto);
    void UpdateAccount(AccountDTO dto);
    void ResetPassword(int employeeId, string newPassword);

    [RequireRole(Role.Administrator)]
    void ChangeRole(int employeeId, int roleId);

    void DeactivateEmployee(int employeeId);
    void ReactivateEmployee(int employeeId);
}