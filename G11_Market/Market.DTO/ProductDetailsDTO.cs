namespace Market.DTO;

public sealed class ProductDetailsDTO
{
    public ProductDTO Product { get; }
    public CategoryDTO Category { get; }
    public IReadOnlyList<ProductDetailAttributeDTO> Attributes { get; }

    public ProductDetailsDTO(
        ProductDTO product,
        CategoryDTO category,
        IReadOnlyList<ProductDetailAttributeDTO> attributes)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(attributes);

        Product = product;
        Category = category;
        Attributes = attributes;
    }
}