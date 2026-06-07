using Market.DTO;

namespace Market.Tests.Helpers;

public static class InventoryManagerTestDataFactory
{
    public static InventoryManagerDTO CreateInventoryManager(
        int employeeId,
        decimal stockAdjustmentLimit = 1000,
        bool canApproveStockCorrection = false,
        bool canApproveNegativeStock = false)
    {
        return new InventoryManagerDTO
        {
            Id = employeeId,
            StockAdjustmentLimit = stockAdjustmentLimit,
            CanApproveStockCorrection = canApproveStockCorrection,
            CanApproveNegativeStock = canApproveNegativeStock,
            IsDeleted = false
        };
    }
}