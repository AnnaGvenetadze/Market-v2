using System.Collections.Immutable;
using Market.DTO;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Services;
using Serilog;

namespace Market.Services;

public class ProductService : IProductService
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    public ProductService(IUnitOfWork unitOfWork, ILogger logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public ProductDTO CreateProduct(ProductDTO product, IEnumerable<ProductAttributeValueDTO>? attributeValues = null)
    {
        ArgumentNullException.ThrowIfNull(product);
        if (product.CategoryId <= 0)
            throw new ArgumentOutOfRangeException(nameof(product.CategoryId), "Category is required.");
        ArgumentException.ThrowIfNullOrWhiteSpace(product.ProductName);

        var category = _unitOfWork.CategoryRepository.GetById(product.CategoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.Warning("Failed to create product: CategoryId {CategoryId} does not exist or is deleted", product.CategoryId);
            throw new InvalidOperationException($"Category with ID {product.CategoryId} does not exist.");
        }

        int productId = _unitOfWork.ProductRepository.Insert(product);
        if (attributeValues != null)
        {
            foreach (var attribute in attributeValues)
            {
                attribute.ProductId = productId;
                _unitOfWork.ProductRepository.InsertAttributeValue(attribute);
            }
        }
        var createdProduct = _unitOfWork.ProductRepository.GetById(productId);
        if (createdProduct == null)
        {
            _logger.Error("Product {ProductId} was inserted but could not be retrieved", productId);
            throw new InvalidOperationException($"Product with ID {productId} was not found after creation.");
        }
        return createdProduct;
    }

    public ProductDTO UpdateProduct(ProductDTO product, IEnumerable<ProductAttributeValueDTO>? attributeValues = null)
    {
        ArgumentNullException.ThrowIfNull(product);
        if (product.Id <= 0)
            throw new ArgumentOutOfRangeException(nameof(product.Id), "Product ID must be greater than zero.");
        if (product.CategoryId <= 0)
            throw new ArgumentOutOfRangeException(nameof(product.CategoryId), "Category ID is required.");
        ArgumentException.ThrowIfNullOrWhiteSpace(product.ProductName);
        var existingProduct = _unitOfWork.ProductRepository.GetById(product.Id);
        if (existingProduct == null)
        {
            _logger.Warning("Update failed: ProductId {ProductId} does not exist", product.Id);
            throw new InvalidOperationException($"Product with ID {product.Id} does not exist.");
        }

        _unitOfWork.ProductRepository.Update(product);

        if (attributeValues != null)
        {
            var existingAttributes = _unitOfWork.ProductRepository
                .GetAttributeValues(product.Id).ToImmutableList();

            foreach (var attribute in attributeValues)
            {
                attribute.ProductId = product.Id;
                if (existingAttributes.Exists(a => a.AttributeId == attribute.AttributeId))
                {
                    _unitOfWork.ProductRepository.UpdateAttributeValue(attribute);
                }
                else
                {
                    _unitOfWork.ProductRepository.InsertAttributeValue(attribute);
                }
            }
        }

        return _unitOfWork.ProductRepository.GetById(product.Id)
            ?? throw new InvalidOperationException($"Product with ID {product.Id} was not found after update.");
    }

    public void DeleteProduct(int productId)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");
        _unitOfWork.ProductRepository.Delete(productId);
    }

    public void RestoreProduct(int productId)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");
        _unitOfWork.ProductRepository.Restore(productId);
    }

    public ProductDTO? GetProductById(int productId)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");
        var product = _unitOfWork.ProductRepository.GetById(productId);
        if (product == null)
            _logger.Warning("ProductId {ProductId} was not found", productId);
        return product;
    }

    public IEnumerable<ProductDTO> GetAllProducts()
    {
        return _unitOfWork.ProductRepository.GetAll();
    }

    public IEnumerable<ProductDTO> GetProductsByCategoryId(int categoryId)
    {
        if (categoryId <= 0)
            throw new ArgumentOutOfRangeException(nameof(categoryId), "Category ID must be greater than zero.");
        return _unitOfWork.ProductRepository.GetByCategoryId(categoryId);
    }

    public IEnumerable<ProductDTO> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
    {
        if (minPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(minPrice), "Minimum price cannot be negative.");
        if (maxPrice < minPrice)
            throw new ArgumentException("Maximum price must be greater than or equal to minimum price.", nameof(maxPrice));
        return _unitOfWork.ProductRepository.GetByPriceRange(minPrice, maxPrice);
    }

    public IEnumerable<ProductAttributeValueDTO> GetProductAttributeValues(int productId)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");
        return _unitOfWork.ProductRepository.GetAttributeValues(productId);
    }

    public ProductDetailsDTO? GetProductDetails(int productId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(productId);

        var details = _unitOfWork.ProductRepository.GetProductDetails(productId);

        if (details is null)
        {
            _logger.Warning(
                "Product with ID {ProductId} was not found.",
                productId);

            return null;
        }
        if (details.Product.IsDeleted)
        {
            _logger.Information(
                "Product with ID {ProductId} is inactive.",
                productId);
        }

        return details;
    }

    public ProductDTO? GetProductByName(string productName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);

        return _unitOfWork.ProductRepository.GetByName(productName);
    }

    public void AddProductAttributeValue(ProductAttributeValueDTO attributeValue)
    {
        ArgumentNullException.ThrowIfNull(attributeValue);
        if (attributeValue.ProductId <= 0)
            throw new ArgumentOutOfRangeException(nameof(attributeValue.ProductId), "Product ID must be greater than zero.");
        if (attributeValue.AttributeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(attributeValue.AttributeId), "Attribute ID must be greater than zero.");
        _unitOfWork.ProductRepository.InsertAttributeValue(attributeValue);
    }

    public void UpdateProductAttributeValue(ProductAttributeValueDTO attributeValue)
    {
        ArgumentNullException.ThrowIfNull(attributeValue);
        if (attributeValue.ProductId <= 0)
            throw new ArgumentOutOfRangeException(nameof(attributeValue.ProductId), "Product ID must be greater than zero.");
        if (attributeValue.AttributeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(attributeValue.AttributeId), "Attribute ID must be greater than zero.");
        _unitOfWork.ProductRepository.UpdateAttributeValue(attributeValue);
    }

    public void DeleteProductAttributeValue(int productId, int attributeId)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");
        if (attributeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(attributeId), "Attribute ID must be greater than zero.");
        _unitOfWork.ProductRepository.DeleteAttributeValue(productId, attributeId);
    }

}