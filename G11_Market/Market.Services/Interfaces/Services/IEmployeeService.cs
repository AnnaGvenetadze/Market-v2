using Market.DTO;
using Market.Services.Attributes;
using Market.Services.Enums;

namespace Market.Services.Interfaces.Services;

public interface IEmployeeService
{
    [RequireRole(Role.Administrator)]
    EmployeeDTO? GetById(int employeeId);

    [RequireRole(Role.Administrator)]
    IEnumerable<EmployeeDTO> GetAll();

    [RequireRole(Role.Administrator)]
    EmployeeDTO CreateEmployee(CreateEmployeeDTO dto);

    [RequireRole(Role.Administrator)]
    void UpdateEmployee(EmployeeDTO dto);

    [RequireRole(Role.Administrator)]
    void UpdateAccount(AccountDTO dto);

    [RequireRole(Role.Administrator)]
    void ResetPassword(int employeeId, string newPassword);

    [RequireRole(Role.Administrator)]
    void ChangeRole(int employeeId, int roleId);

    [RequireRole(Role.Administrator)]
    void DeactivateEmployee(int employeeId);

    [RequireRole(Role.Administrator)]
    void ReactivateEmployee(int employeeId);
}