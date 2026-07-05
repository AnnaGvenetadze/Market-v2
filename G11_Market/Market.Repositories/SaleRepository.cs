using System.Data;
using System.Data.Common;
using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class SaleRepository(DbConnection connection) : BaseRepository<SaleDTO>(connection), ISaleRepository
{
    private readonly DbConnection _connection = connection;

    public IEnumerable<SaleDTO> GetSalesByEmployee(int employeeId) => Search(s => s.CreatedEmployeeId == employeeId);
    public IEnumerable<SaleDTO> GetCompletedSales() => Search(s => s.Status == 1);
    public void Cancel(int id, int employeeId, string cancelReason)
    {
        _connection.Execute(
            "sp_DeleteSale",
            new
            {
                Id = id,
                EmployeeId = employeeId,
                CancelReason = cancelReason
            },
            commandType: CommandType.StoredProcedure);
    }
}