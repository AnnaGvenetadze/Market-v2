using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class StockMovementDTO
{
    [IgnoreOnInsert]
    public int Id { get; set; }

    public int ProductId { get; set; }
    public byte MovementType { get; set; }
    public int QuantityChange { get; set; }
    public int QuantityBefore { get; set; }
    public int? SaleItemId { get; set; }
    public int ChangedByEmployeeId { get; set; }
    public string? Reason { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime CreatedDate { get; set; }
}