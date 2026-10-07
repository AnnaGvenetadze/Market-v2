using Market.DTO;

public interface IEmployeeService
{
    EmployeeDTO? GetById(int employeeId);
    IEnumerable<EmployeeDTO> GetAll();
    EmployeeDTO CreateEmployee(CreateEmployeeDTO dto);
    void UpdateEmployee(EmployeeDTO dto);
    void UpdateAccount(AccountDTO dto);
    // optional
    void ResetPassword(int employeeId, string newPassword);
    void ChangeRole(int employeeId, int roleId);
    void DeactivateEmployee(int employeeId);
    void ReactivateEmployee(int employeeId);
}