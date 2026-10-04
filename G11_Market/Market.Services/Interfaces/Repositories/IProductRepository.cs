using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IProductRepository : IBaseRepository<ProductDTO>
{
    ProductDTO? GetByName(string productName);
    IEnumerable<ProductDTO> GetByCategoryId(int categoryId);
    IEnumerable<ProductDTO> GetByPriceRange(decimal minPrice, decimal maxPrice);
    // სერვისის ინტერფეისიდან გვინდა ჩანდეს?
    IEnumerable<ProductAttributeValueDTO> GetAttributeValues(int productId);
    ProductDetailsDTO? GetProductDetails(int productId);
    IEnumerable<ProductDTO> GetAllProducts(bool isDeleted = false);
    void InsertAttributeValue(ProductAttributeValueDTO value);
    void UpdateAttributeValue(ProductAttributeValueDTO value);
    void DeleteAttributeValue(int productId, int attributeId);
}
