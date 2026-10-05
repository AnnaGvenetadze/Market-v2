using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class StockDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
}