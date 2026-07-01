using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IStockMovementRepository : IBaseRepository<StockMovementDTO>
{
    IEnumerable<StockMovementDTO> GetByProductId(int productId);
    IEnumerable<StockMovementDTO> GetBySaleItemId(int saleItemId);
    IEnumerable<StockMovementDTO> GetByEmployeeId(int employeeId);
    IEnumerable<StockMovementDTO> GetByMovementType(byte movementType);
}