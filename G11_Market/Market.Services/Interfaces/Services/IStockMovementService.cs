using Market.DTO;
using Market.Services.Attributes;
using Market.Services.Enums;

namespace Market.Services.Interfaces.Services;

public interface IStockMovementService
{
    [RequireRole(Role.InventoryManager, Role.Administrator)]
    IEnumerable<StockMovementDTO> GetByProductId(int productId);

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    IEnumerable<StockMovementDTO> GetByDateRange(DateTime from, DateTime to);

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    IEnumerable<StockMovementDTO> GetByEmployeeId(int employeeId);

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    IEnumerable<StockDTO> GetOutOfStockProducts();

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    void Refill(int productId, int quantity);

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    void Adjust(int productId, int quantityDifference, string? reason);
}