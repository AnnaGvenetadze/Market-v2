using Market.DTO;

public interface IStockMovementService
{
    IEnumerable<StockMovementDTO> GetByProductId(int productId);
    IEnumerable<StockMovementDTO> GetByDateRange(DateTime from,DateTime to);
    IEnumerable<StockMovementDTO> GetByEmployeeId(int employeeId);
    IEnumerable<StockDTO> GetOutOfStockProducts();
    void Refill(int productId, int quantity, int employeeId);
    void Adjust(int productId, int quantityDifference, int employeeId, string? reason);
}