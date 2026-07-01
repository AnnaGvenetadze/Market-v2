using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IProductRepository : IBaseRepository<ProductDTO>
{
    ProductDTO? GetByName(string productName);
    IEnumerable<ProductDTO> GetByCategoryId(int categoryId);
    IEnumerable<ProductDTO> GetByPriceRange(decimal minPrice, decimal maxPrice);
}
