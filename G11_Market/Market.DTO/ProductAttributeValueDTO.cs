namespace Market.DTO;

public sealed class ProductAttributeValueDTO
{
    public int ProductId { get; set; }
    public int AttributeId { get; set; }
    public string? TextValue { get; set; }
    public decimal? NumberValue { get; set; }
    public DateTime? DateValue { get; set; }
    public bool? BooleanValue { get; set; }
}