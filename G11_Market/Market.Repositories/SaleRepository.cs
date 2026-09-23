using Dapper;
using Market.DTO;
using Market.DTO.Enums;
using Market.Services.Interfaces.Repositories;
using System.Data;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class SaleRepository(DbConnection connection) 
    : BaseRepository<SaleDTO>(connection), ISaleRepository
{
    public IEnumerable<SaleDTO> GetSalesByEmployee(int employeeId) => Search(s => s.CreatedEmployeeId == employeeId);
    public IEnumerable<SaleDTO> GetCompletedSales()
        => Search(s => s.Status == SaleStatus.Completed);
    public IEnumerable<SaleDTO> GetSalesByStatus(SaleStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        return Search(s => s.Status == status);
    }

    public IEnumerable<SaleDTO> GetSalesByDateRange(
        DateTime dateFrom,
        DateTime dateTo)
    {
        if (dateFrom > dateTo)
            throw new ArgumentException(
                "DateFrom cannot be greater than DateTo.");

        return Search(s =>
            s.CreatedDate >= dateFrom &&
            s.CreatedDate <= dateTo);
    }

    public void CancelSale(int id, int employeeId, string cancelReason)
    {
        _connection.Execute(
            "sp_CancelSale",
            new
            {
                Id = id,
                EmployeeId = employeeId,
                CancelReason = cancelReason
            },
            commandType: CommandType.StoredProcedure);
    }
}