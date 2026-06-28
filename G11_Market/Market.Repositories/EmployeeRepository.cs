using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

public sealed class EmployeeRepository(DbConnection connection) : BaseRepository<EmployeeDTO>(connection), IEmployeeRepository
{
    public EmployeeDTO? GetByEmployeeCode(string employeeCode) => Search(e => e.EmployeeCode == employeeCode).FirstOrDefault();

    public EmployeeDTO? GetByAccountId(int accountId) => Search(e => e.AccountId == accountId).FirstOrDefault();

    public IEnumerable<EmployeeDTO> GetSubordinates(int managerEmployeeId) => Search(e => e.ManagerEmployeeId == managerEmployeeId);

    public IEnumerable<EmployeeDTO> GetAllActive() => Search(e => e.IsDeleted == false);
}