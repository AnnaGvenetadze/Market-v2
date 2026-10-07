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
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));

        return Search(x => x.ProductId == productId)
            .OrderByDescending(x => x.CreatedDate);
    }

    public IEnumerable<StockMovementDTO> GetByDateRange(DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("From date must be less than or equal to To date.");

        return Search(x => x.CreatedDate >= from && x.CreatedDate <= to)
            .OrderByDescending(x => x.CreatedDate);
    }

    public IEnumerable<StockMovementDTO> GetByEmployeeId(int employeeId)
    {
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        return Search(x => x.ChangedByEmployeeId == employeeId)
            .OrderByDescending(x => x.CreatedDate);
    }

    public IEnumerable<StockDTO> GetOutOfStockProducts()
    {
        return connection.Query<StockDTO>(
            "dbo.sp_GetOutOfStockProducts",
            commandType: CommandType.StoredProcedure);
    }

    public void Refill(int productId, int quantity, int employeeId)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        connection.Execute(
            "dbo.sp_RefillStock",
            new
            {
                ProductId = productId,
                Quantity = quantity,
                ChangedByEmployeeId = employeeId
            },
            commandType: CommandType.StoredProcedure);
    }

    public void Adjust(int productId, int quantityDifference, int employeeId, string? reason)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantityDifference == 0)
            throw new ArgumentOutOfRangeException(nameof(quantityDifference));
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required.", nameof(reason));

        connection.Execute(
            "dbo.sp_AdjustStock",
            new
            {
                ProductId = productId,
                QuantityDifference = quantityDifference,
                ChangedByEmployeeId = employeeId,
                Reason = reason.Trim()
            },
            commandType: CommandType.StoredProcedure);
    }
}