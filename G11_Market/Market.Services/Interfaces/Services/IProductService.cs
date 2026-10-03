using Market.DTO;

namespace Market.Services.Interfaces.Services;

public interface IProductService
{
    ProductDTO CreateProduct(ProductDTO product, IEnumerable<ProductAttributeValueDTO>? attributeValues = null);
    ProductDTO UpdateProduct(ProductDTO product, IEnumerable<ProductAttributeValueDTO>? attributeValues = null);
    void DeleteProduct(int productId);
    void RestoreProduct(int productId);
    ProductDTO? GetProductById(int productId);
    IEnumerable<ProductDTO> GetAllProducts();
    IEnumerable<ProductDTO> GetProductsByCategoryId(int categoryId);
    IEnumerable<ProductDTO> GetProductsByPriceRange(decimal minPrice, decimal maxPrice);
    IEnumerable<ProductAttributeValueDTO> GetProductAttributeValues(int productId);
    ProductDetailsDTO? GetProductDetails(int productId);
    void AddProductAttributeValue(ProductAttributeValueDTO attributeValue);
    void UpdateProductAttributeValue(ProductAttributeValueDTO attributeValue);
    void DeleteProductAttributeValue(int productId, int attributeId);
}