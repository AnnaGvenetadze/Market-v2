namespace Market.DTO;

public sealed class ProductDetailsDTO
{
    public required ProductDTO Product { get; init; }
    public required CategoryDTO Category { get; init; }
    public required IReadOnlyList<ProductDetailAttributeDTO> Attributes { get; init; }
}