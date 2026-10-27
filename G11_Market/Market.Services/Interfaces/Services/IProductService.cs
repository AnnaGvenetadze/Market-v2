using Market.DTO;
using Market.Services.Attributes;
using Market.Services.Enums;

namespace Market.Services.Interfaces.Services;

public interface IProductService
{
    [RequireRole(Role.InventoryManager, Role.Administrator)]
    int CreateProduct(ProductDTO product, IEnumerable<ProductAttributeValueDTO>? attributeValues = null);

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    int UpdateProduct(ProductDTO product, IEnumerable<ProductAttributeValueDTO>? attributeValues = null);

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    void DeleteProduct(int productId);

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    void RestoreProduct(int productId);

    ProductDTO? GetProductById(int productId);

    public ProductDTO? GetProductByName(string productName);

    IEnumerable<ProductDTO> GetProductsByCategoryId(int categoryId);

    IEnumerable<ProductDTO> GetProductsByPriceRange(decimal minPrice, decimal maxPrice);

    IEnumerable<ProductAttributeValueDTO> GetProductAttributeValues(int productId);

    ProductDetailsDTO? GetProductDetails(int productId);

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    void AddProductAttributeValue(ProductAttributeValueDTO attributeValue);

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    void UpdateProductAttributeValue(ProductAttributeValueDTO attributeValue);

    [RequireRole(Role.InventoryManager, Role.Administrator)]
    void DeleteProductAttributeValue(int productId, int attributeId);
}