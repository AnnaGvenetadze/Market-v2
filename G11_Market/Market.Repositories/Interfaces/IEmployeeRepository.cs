using Market.DTO;

namespace Market.Repositories.Interfaces;

public interface IEmployeeRepository
{
    EmployeeDTO? GetByEmployeeCode(string employeeCode);
    EmployeeDTO? GetByAccountId(int accountId);
    IEnumerable<EmployeeDTO> GetSubordinates(int managerEmployeeId);
    IEnumerable<EmployeeDTO> GetAllActive();
}
