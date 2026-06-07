using Market.Extensions.Attributes;

namespace Market.DTO;

public class InventoryManagerDTO
{
    public int Id { get; set; }

    public decimal StockAdjustmentLimit { get; set; }

    public bool CanApproveStockCorrection { get; set; }

    public bool CanApproveNegativeStock { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public bool IsDeleted { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime CreateDate { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime? UpdateDate { get; set; }
}