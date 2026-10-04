using Dapper;
using Market.DTO;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Repositories;
using System.Data;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class ProductRepository(DbConnection connection)
    : BaseRepository<ProductDTO>(connection), IProductRepository
{
    public ProductDTO? GetByName(string productName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);
        productName = productName.Trim();

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

    public void InsertAttributeValue(ProductAttributeValueDTO value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _connection.Execute(
            "dbo.sp_InsertProductAttributeValue",
            value,
            commandType: CommandType.StoredProcedure);
    }

    public void UpdateAttributeValue(ProductAttributeValueDTO value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _connection.Execute(
            "dbo.sp_UpdateProductAttributeValue",
            value,
            commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<ProductAttributeValueDTO> GetAttributeValues(int productId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(productId);

        var parameters = new DynamicParameters();
        parameters.Add("ProductId", productId);

        return _connection.Query<ProductAttributeValueDTO>(
            "dbo.sp_GetProductAttributeValuesByProductId",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public ProductDetailsDTO? GetProductDetails(int productId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(productId);

        using var multi = _connection.QueryMultiple(
            "dbo.sp_GetProductDetails",
            new { ProductId = productId },
            commandType: CommandType.StoredProcedure);

        var product = multi.ReadFirstOrDefault<ProductDTO>();
        var category = multi.ReadFirstOrDefault<CategoryDTO>();
        var attributes = multi.Read<ProductDetailAttributeDTO>().ToList();

        if (product is null || category is null)
            return null;

        return new ProductDetailsDTO(product, category, attributes);
    }

    public void DeleteAttributeValue(int productId, int attributeId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(productId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(attributeId);

        var parameters = new DynamicParameters();
        parameters.Add("ProductId", productId);
        parameters.Add("AttributeId", attributeId);

        _connection.Execute(
            "dbo.sp_DeleteProductAttributeValue",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
}