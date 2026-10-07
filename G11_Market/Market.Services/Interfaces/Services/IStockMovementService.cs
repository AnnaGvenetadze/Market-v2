using Market.DTO;

namespace Market.Services.Interfaces.Services;

public interface IStockMovementService
{
    IEnumerable<StockMovementDTO> GetByProductId(int productId);
    IEnumerable<StockMovementDTO> GetByDateRange(DateTime from, DateTime to);
    IEnumerable<StockMovementDTO> GetByEmployeeId(int employeeId);
    IEnumerable<StockDTO> GetOutOfStockProducts();

    void Refill(int productId, int quantity);
    void Adjust(int productId, int quantityDifference, string? reason);
}