using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

public sealed class ProductRepository(DbConnection connection)
    : BaseRepository<ProductDTO>(connection), IProductRepository
{
    public ProductDTO? GetByName(string productName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);

        return Search(product =>
                product.ProductName == productName &&
                product.IsDeleted == false)
            .FirstOrDefault();
    }

    public IEnumerable<ProductDTO> GetByCategoryId(int categoryId)
    {
        if (categoryId <= 0)
            throw new ArgumentOutOfRangeException(nameof(categoryId));

        return Search(product =>
            product.CategoryId == categoryId &&
            product.IsDeleted == false);
    }

    public IEnumerable<ProductDTO> GetByPriceRange(decimal minPrice, decimal maxPrice)
    {
        if (minPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(minPrice));

        if (maxPrice < minPrice)
            throw new ArgumentException("Max price must be greater than or equal to min price.");

        return Search(product =>
            product.Price >= minPrice &&
            product.Price <= maxPrice &&
            product.IsDeleted == false);
    }
}