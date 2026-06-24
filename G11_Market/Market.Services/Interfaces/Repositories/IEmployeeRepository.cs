using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IEmployeeRepository
{
    EmployeeDTO? GetByEmployeeCode(string employeeCode);
    EmployeeDTO? GetByAccountId(int accountId);
    IEnumerable<EmployeeDTO> GetSubordinates(int managerEmployeeId);
    IEnumerable<EmployeeDTO> GetAllActive();
}
