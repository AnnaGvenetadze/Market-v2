using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class SaleItemDTO
{
    [IgnoreOnInsert]
    public int Id { get; set; }

    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountAmount { get; set; }
}