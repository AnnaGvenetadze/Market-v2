using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class StockMovementRepository(DbConnection connection)
    : BaseRepository<StockMovementDTO>(connection), IStockMovementRepository
{
    public IEnumerable<StockMovementDTO> GetByProductId(int productId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(productId);

        return Search(movement => movement.ProductId == productId);
    }


    public IEnumerable<StockMovementDTO> GetByEmployeeId(int employeeId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(employeeId);

        return Search(movement => movement.ChangedByEmployeeId == employeeId);
    }


    public IEnumerable<StockMovementDTO> GetByDateRange(DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("From date cannot be greater than to date.");

        return Search(movement =>
            movement.CreatedDate >= from && movement.CreatedDate <= to);
    }


    public IEnumerable<StockDTO> GetOutOfStockProducts()
    {
        return _connection.Query<StockDTO>(
            "dbo.sp_GetOutOfStockProducts",
            commandType: CommandType.StoredProcedure);
    }


    public void Refill(int productId, int quantity, int employeeId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(productId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(employeeId);

        var parameters = new DynamicParameters();
        parameters.Add("ProductId", productId);
        parameters.Add("Quantity", quantity);
        parameters.Add("EmployeeId", employeeId);

        _connection.Execute(
            "dbo.sp_RefillStock",
            parameters,
            commandType: CommandType.StoredProcedure);
    }


    public void Adjust(int productId, int quantityDifference, int employeeId, string? reason)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(productId);
        if (quantityDifference == 0)
            throw new ArgumentException("Quantity difference cannot be zero.", nameof(quantityDifference));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(employeeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        
        reason = reason.Trim();
        var parameters = new DynamicParameters();

        parameters.Add("ProductId", productId);
        parameters.Add("QuantityDifference", quantityDifference);
        parameters.Add("EmployeeId", employeeId);
        parameters.Add("Reason", reason);

        _connection.Execute(
            "dbo.sp_AdjustStock",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
}