using System.Data.Common;
using Market.DTO;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public sealed class SaleRepository(DbConnection connection) : BaseRepository<SaleDTO>(connection), ISaleRepository
{
    public IEnumerable<SaleDTO> GetSalesByEmployee(int employeeId) => Search(s => s.CreatedEmployeeId == employeeId);
    public IEnumerable<SaleDTO> GetCompletedSales() => Search(s => s.Status == 1);
}