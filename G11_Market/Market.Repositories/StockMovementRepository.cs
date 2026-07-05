using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class StockMovementRepository(DbConnection connection)
    : BaseRepository<StockMovementDTO>(connection), IStockMovementRepository
{
    public IEnumerable<StockMovementDTO> GetByProductId(int productId)
        => Search(movement => movement.ProductId == productId);

    public IEnumerable<StockMovementDTO> GetBySaleItemId(int saleItemId)
        => Search(movement => movement.SaleItemId == saleItemId);

    public IEnumerable<StockMovementDTO> GetByEmployeeId(int employeeId)
        => Search(movement => movement.ChangedByEmployeeId == employeeId);

    public IEnumerable<StockMovementDTO> GetByMovementType(byte movementType)
        => Search(movement => movement.MovementType == movementType);
}