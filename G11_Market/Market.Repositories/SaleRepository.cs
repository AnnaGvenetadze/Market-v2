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
    public IEnumerable<SaleDTO> GetSalesByEmployee(int employeeId)
    {
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        return Search(s => s.CreatedEmployeeId == employeeId);
    }

    public IEnumerable<SaleDTO> GetSalesByStatus(SaleStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        return Search(s => s.Status == status);
    }

    public IEnumerable<SaleDTO> GetSalesByDateRange(DateTime dateFrom, DateTime dateTo)
    {
        if (dateFrom > dateTo)
            throw new ArgumentException("DateFrom cannot be greater than DateTo.");

        return Search(s => s.CreatedDate >= dateFrom && s.CreatedDate <= dateTo);
    }

    public void CompleteSale(int saleId)
    {
        if (saleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleId));

        _connection.Execute(
            "sp_CompleteSale",
            new
            {
                SaleId = saleId
            },
            commandType: CommandType.StoredProcedure);
    }

    public void CancelSale(int saleId, int employeeId, string cancelReason)
    {
        if (saleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleId));
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));
        ArgumentException.ThrowIfNullOrWhiteSpace(cancelReason);

        _connection.Execute(
            "sp_CancelSale",
            new
            {
                Id = saleId,
                EmployeeId = employeeId,
                CancelReason = cancelReason
            },
            commandType: CommandType.StoredProcedure);
    }

    public decimal GetIncomeByDateRange(DateTime dateFrom, DateTime dateTo)
    {
        if (dateFrom > dateTo)
            throw new ArgumentException("DateFrom cannot be greater than DateTo.");

        return _connection.QuerySingle<decimal>(
            "sp_GetIncomeByDateRange",
            new
            {
                DateFrom = dateFrom,
                DateTo = dateTo
            },
            commandType: CommandType.StoredProcedure);
    }
}