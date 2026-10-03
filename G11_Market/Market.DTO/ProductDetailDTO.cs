namespace Market.DTO;

public sealed class ProductDetailAttributeDTO
{
    public int AttributeId { get; set; }
    public string AttributeName { get; set; } = null!;
    public byte AttributeType { get; set; }
    public int OrderPosition { get; set; }

    public string? TextValue { get; set; }
    public decimal? NumberValue { get; set; }
    public DateTime? DateValue { get; set; }
    public bool? BooleanValue { get; set; }
}