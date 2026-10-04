using Market.DTO;
using Market.Services;
using Market.Services.Interfaces.Services;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Market.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ProductServiceTests : BaseRepositoryTests
{
    // =========================================================
    // Constructor
    // =========================================================

    [Test]
    public void Constructor_WhenUnitOfWorkIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ProductService(null!, Log.Logger));
    }

    [Test]
    public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ProductService(UnitOfWork, null!));
    }


    // =========================================================
    // CreateProduct
    // =========================================================

    [Test]
    public void CreateProduct_WithValidProduct_ShouldCreateProduct()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "New Product".AddGuid(),
            Price = 500m
        };

        // Act
        var result = service.CreateProduct(product);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.CategoryId, Is.EqualTo(1));
            Assert.That(result.ProductName, Is.EqualTo(product.ProductName));
            Assert.That(result.Price, Is.EqualTo(500m));
            Assert.That(result.IsDeleted, Is.False);
        });
    }

    [Test]
    public void CreateProduct_WithAttributeValue_ShouldCreateProductWithAttributeValue()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "LG Monitor".AddGuid(),
            Price = 1500m
        };

        var attributes = new List<ProductAttributeValueDTO>
        {
            new()
            {
                AttributeId = 1,
                TextValue = "LG"
            }
        };

        // Act
        var createdProduct = service.CreateProduct(
            product,
            attributes);

        var result = service
            .GetProductAttributeValues(createdProduct.Id)
            .ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));

        var attribute = result.Single();

        Assert.Multiple(() =>
        {
            Assert.That(attribute.ProductId,
                Is.EqualTo(createdProduct.Id));

            Assert.That(attribute.AttributeId,
                Is.EqualTo(1));

            Assert.That(attribute.TextValue,
                Is.EqualTo("LG"));

            Assert.That(attribute.NumberValue, Is.Null);
            Assert.That(attribute.DateValue, Is.Null);
            Assert.That(attribute.BooleanValue, Is.Null);
        });
    }

    [Test]
    public void CreateProduct_WhenProductIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.CreateProduct(null!));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void CreateProduct_WhenCategoryIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int categoryId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            CategoryId = categoryId,
            ProductName = "Product".AddGuid(),
            Price = 100m
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.CreateProduct(product));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void CreateProduct_WhenProductNameIsInvalid_ShouldThrowArgumentException(
        string productName)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = productName,
            Price = 100m
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.CreateProduct(product));
    }

    [Test]
    public void CreateProduct_WhenCategoryDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            CategoryId = int.MaxValue,
            ProductName = "Product".AddGuid(),
            Price = 100m
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.CreateProduct(product));
    }

    [Test]
    public void CreateProduct_WhenCategoryIsDeleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        UnitOfWork.CategoryRepository.Delete(1);

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "Product".AddGuid(),
            Price = 100m
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.CreateProduct(product));
    }

    [Test]
    public void CreateProduct_WhenPriceIsNegative_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "Product".AddGuid(),
            Price = -1m
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.CreateProduct(product));
    }

    [Test]
    public void CreateProduct_WhenProductNameAlreadyExists_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "Samsung Monitor",
            Price = 100m
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.CreateProduct(product));
    }

    [Test]
    public void CreateProduct_WhenAttributeValueTypeIsWrong_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "Product".AddGuid(),
            Price = 100m
        };

        // Attribute 1 = Brand = Text
        var attributes = new List<ProductAttributeValueDTO>
        {
            new()
            {
                AttributeId = 1,
                NumberValue = 100m
            }
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.CreateProduct(product, attributes));
    }


    // =========================================================
    // UpdateProduct
    // =========================================================

    [Test]
    public void UpdateProduct_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = service.GetProductById(1)!;

        product.ProductName = "Updated Monitor".AddGuid();
        product.Price = 1400m;

        // Act
        var result = service.UpdateProduct(product);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.ProductName,
                Is.EqualTo(product.ProductName));
            Assert.That(result.Price, Is.EqualTo(1400m));
        });
    }

    [Test]
    public void UpdateProduct_WithExistingAttributeValue_ShouldUpdateAttributeValue()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = service.GetProductById(1)!;

        var attributes = new List<ProductAttributeValueDTO>
        {
            new()
            {
                AttributeId = 1,
                TextValue = "LG"
            }
        };

        // Act
        service.UpdateProduct(product, attributes);

        var result = service
            .GetProductAttributeValues(1)
            .Single();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.AttributeId, Is.EqualTo(1));
            Assert.That(result.TextValue, Is.EqualTo("LG"));
        });
    }

    [Test]
    public void UpdateProduct_WhenAttributeValueDoesNotExist_ShouldInsertAttributeValue()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        service.DeleteProductAttributeValue(1, 1);

        var product = service.GetProductById(1)!;

        var attributes = new List<ProductAttributeValueDTO>
        {
            new()
            {
                AttributeId = 1,
                TextValue = "Sony"
            }
        };

        // Act
        service.UpdateProduct(product, attributes);

        var result = service
            .GetProductAttributeValues(1)
            .Single();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.AttributeId, Is.EqualTo(1));
            Assert.That(result.TextValue, Is.EqualTo("Sony"));
        });
    }

    [Test]
    public void UpdateProduct_WhenProductIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.UpdateProduct(null!));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void UpdateProduct_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            Id = productId,
            CategoryId = 1,
            ProductName = "Product",
            Price = 100m
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.UpdateProduct(product));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void UpdateProduct_WhenCategoryIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int categoryId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            Id = 1,
            CategoryId = categoryId,
            ProductName = "Product",
            Price = 100m
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.UpdateProduct(product));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void UpdateProduct_WhenProductNameIsInvalid_ShouldThrowArgumentException(
        string productName)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = new ProductDTO
        {
            Id = 1,
            CategoryId = 1,
            ProductName = productName,
            Price = 100m
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.UpdateProduct(product));
    }

    [Test]
    public void UpdateProduct_WhenPriceIsNegative_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var product = service.GetProductById(1)!;
        product.Price = -1m;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.UpdateProduct(product));
    }

    [Test]
    public void UpdateProduct_WhenNewCategoryIsNotCompatibleWithAttributes_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Product 1 has Attribute 1.
        // Category 2 has Attribute 2.
        var product = service.GetProductById(1)!;
        product.CategoryId = 2;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.UpdateProduct(product));
    }


    // =========================================================
    // DeleteProduct
    // =========================================================

    [Test]
    public void DeleteProduct_WithValidId_ShouldDeleteProduct()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        service.DeleteProduct(1);

        var products = service
            .GetAllProducts()
            .ToList();

        // Assert
        Assert.That(
            products.Any(x => x.Id == 1),
            Is.False);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void DeleteProduct_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.DeleteProduct(productId));
    }

    [Test]
    public void DeleteProduct_WhenProductDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.DeleteProduct(int.MaxValue));
    }


    // =========================================================
    // RestoreProduct
    // =========================================================

    [Test]
    public void RestoreProduct_WhenProductIsDeleted_ShouldRestoreProduct()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        service.DeleteProduct(1);

        // Act
        service.RestoreProduct(1);

        var result = service
            .GetAllProducts()
            .SingleOrDefault(x => x.Id == 1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.IsDeleted, Is.False);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void RestoreProduct_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.RestoreProduct(productId));
    }


    // =========================================================
    // GetProductById
    // =========================================================

    [Test]
    public void GetProductById_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service.GetProductById(1);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.ProductName,
                Is.EqualTo("Samsung Monitor"));
            Assert.That(result.CategoryId, Is.EqualTo(1));
            Assert.That(result.Price, Is.EqualTo(1200m));
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetProductById_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.GetProductById(productId));
    }

    [Test]
    public void GetProductById_WhenProductDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.GetProductById(int.MaxValue));
    }


    // =========================================================
    // GetAllProducts
    // =========================================================

    [Test]
    public void GetAllProducts_ShouldReturnAllActiveProducts()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service
            .GetAllProducts()
            .ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.All(x => !x.IsDeleted), Is.True);
    }

    [Test]
    public void GetAllProducts_WhenProductIsDeleted_ShouldExcludeDeletedProduct()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        service.DeleteProduct(1);

        // Act
        var result = service
            .GetAllProducts()
            .ToList();

        // Assert
        Assert.That(
            result.Any(x => x.Id == 1),
            Is.False);

        Assert.That(
            result.All(x => !x.IsDeleted),
            Is.True);
    }


    // =========================================================
    // GetProductsByCategoryId
    // =========================================================

    [Test]
    public void GetProductsByCategoryId_ShouldReturnCorrectProducts()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service
            .GetProductsByCategoryId(1)
            .ToList();

        // Assert
        Assert.That(result, Is.Not.Empty);

        Assert.That(
            result.All(x => x.CategoryId == 1),
            Is.True);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetProductsByCategoryId_WhenCategoryIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int categoryId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.GetProductsByCategoryId(categoryId));
    }

    [Test]
    public void GetProductsByCategoryId_WhenNoProductsExist_ShouldReturnEmpty()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service
            .GetProductsByCategoryId(int.MaxValue)
            .ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }


    // =========================================================
    // GetProductsByPriceRange
    // =========================================================

    [Test]
    public void GetProductsByPriceRange_ShouldReturnProductsInsideRange()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service
            .GetProductsByPriceRange(1200m, 2500m)
            .ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));

        Assert.That(
            result.All(x =>
                x.Price >= 1200m &&
                x.Price <= 2500m),
            Is.True);
    }

    [Test]
    public void GetProductsByPriceRange_ShouldIncludeBoundaryValues()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service
            .GetProductsByPriceRange(1200m, 1200m)
            .ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Single().Id, Is.EqualTo(1));
    }

    [Test]
    public void GetProductsByPriceRange_WhenNoProductsExist_ShouldReturnEmpty()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service
            .GetProductsByPriceRange(10000m, 20000m)
            .ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetProductsByPriceRange_WhenMinimumPriceIsNegative_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.GetProductsByPriceRange(-1m, 100m));
    }

    [Test]
    public void GetProductsByPriceRange_WhenMaximumPriceIsLessThanMinimum_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.GetProductsByPriceRange(200m, 100m));
    }


    // =========================================================
    // GetProductAttributeValues
    // =========================================================

    [Test]
    public void GetProductAttributeValues_ShouldReturnCorrectValues()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service
            .GetProductAttributeValues(1)
            .ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));

        var attribute = result.Single();

        Assert.Multiple(() =>
        {
            Assert.That(attribute.ProductId, Is.EqualTo(1));
            Assert.That(attribute.AttributeId, Is.EqualTo(1));
            Assert.That(attribute.TextValue,
                Is.EqualTo("Samsung"));
            Assert.That(attribute.NumberValue, Is.Null);
            Assert.That(attribute.DateValue, Is.Null);
            Assert.That(attribute.BooleanValue, Is.Null);
        });
    }

    [Test]
    public void GetProductAttributeValues_WhenProductHasNoValues_ShouldReturnEmpty()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        service.DeleteProductAttributeValue(1, 1);

        // Act
        var result = service
            .GetProductAttributeValues(1)
            .ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetProductAttributeValues_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.GetProductAttributeValues(productId));
    }

    [Test]
    public void GetProductAttributeValues_WhenProductDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.GetProductAttributeValues(int.MaxValue).ToList());
    }


    // =========================================================
    // AddProductAttributeValue
    // =========================================================

    [Test]
    public void AddProductAttributeValue_WithValidValue_ShouldAddValue()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        service.DeleteProductAttributeValue(1, 1);

        var value = new ProductAttributeValueDTO
        {
            ProductId = 1,
            AttributeId = 1,
            TextValue = "LG"
        };

        // Act
        service.AddProductAttributeValue(value);

        var result = service
            .GetProductAttributeValues(1)
            .Single();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ProductId, Is.EqualTo(1));
            Assert.That(result.AttributeId, Is.EqualTo(1));
            Assert.That(result.TextValue, Is.EqualTo("LG"));
        });
    }

    [Test]
    public void AddProductAttributeValue_WhenValueIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddProductAttributeValue(null!));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void AddProductAttributeValue_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var value = new ProductAttributeValueDTO
        {
            ProductId = productId,
            AttributeId = 1,
            TextValue = "LG"
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddProductAttributeValue(value));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void AddProductAttributeValue_WhenAttributeIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int attributeId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var value = new ProductAttributeValueDTO
        {
            ProductId = 1,
            AttributeId = attributeId,
            TextValue = "LG"
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddProductAttributeValue(value));
    }

    [Test]
    public void AddProductAttributeValue_WhenValueTypeIsWrong_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        service.DeleteProductAttributeValue(1, 1);

        var value = new ProductAttributeValueDTO
        {
            ProductId = 1,
            AttributeId = 1,
            NumberValue = 10m
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.AddProductAttributeValue(value));
    }

    [Test]
    public void AddProductAttributeValue_WhenAttributeDoesNotBelongToCategory_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var value = new ProductAttributeValueDTO
        {
            ProductId = 1,
            AttributeId = 2,
            NumberValue = 2.5m
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.AddProductAttributeValue(value));
    }


    // =========================================================
    // UpdateProductAttributeValue
    // =========================================================

    [Test]
    public void UpdateProductAttributeValue_WithValidValue_ShouldUpdateValue()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var value = new ProductAttributeValueDTO
        {
            ProductId = 1,
            AttributeId = 1,
            TextValue = "LG"
        };

        // Act
        service.UpdateProductAttributeValue(value);

        var result = service
            .GetProductAttributeValues(1)
            .Single();

        // Assert
        Assert.That(result.TextValue, Is.EqualTo("LG"));
    }

    [Test]
    public void UpdateProductAttributeValue_WhenValueIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.UpdateProductAttributeValue(null!));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void UpdateProductAttributeValue_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var value = new ProductAttributeValueDTO
        {
            ProductId = productId,
            AttributeId = 1,
            TextValue = "LG"
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.UpdateProductAttributeValue(value));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void UpdateProductAttributeValue_WhenAttributeIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int attributeId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var value = new ProductAttributeValueDTO
        {
            ProductId = 1,
            AttributeId = attributeId,
            TextValue = "LG"
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.UpdateProductAttributeValue(value));
    }

    [Test]
    public void UpdateProductAttributeValue_WhenValueDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        service.DeleteProductAttributeValue(1, 1);

        var value = new ProductAttributeValueDTO
        {
            ProductId = 1,
            AttributeId = 1,
            TextValue = "LG"
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.UpdateProductAttributeValue(value));
    }

    [Test]
    public void UpdateProductAttributeValue_WhenValueTypeIsWrong_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        var value = new ProductAttributeValueDTO
        {
            ProductId = 1,
            AttributeId = 1,
            NumberValue = 10m
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.UpdateProductAttributeValue(value));
    }


    // =========================================================
    // DeleteProductAttributeValue
    // =========================================================

    [Test]
    public void DeleteProductAttributeValue_WhenValueExists_ShouldDeleteValue()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        service.DeleteProductAttributeValue(1, 1);

        var result = service
            .GetProductAttributeValues(1)
            .ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void DeleteProductAttributeValue_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.DeleteProductAttributeValue(
                productId,
                1));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void DeleteProductAttributeValue_WhenAttributeIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int attributeId)
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.DeleteProductAttributeValue(
                1,
                attributeId));
    }

    [Test]
    public void DeleteProductAttributeValue_WhenProductDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.DeleteProductAttributeValue(
                int.MaxValue,
                1));
    }

    [Test]
    public void DeleteProductAttributeValue_WhenAttributeDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.DeleteProductAttributeValue(
                1,
                int.MaxValue));
    }

    // =========================================================
    // GetProductDetails
    // =========================================================

    [Test]
    public void GetProductDetails_WhenProductExists_ShouldReturnCorrectDetails()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service.GetProductDetails(1);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(result!.Product.Id, Is.EqualTo(1));
            Assert.That(result.Product.ProductName,
                Is.EqualTo("Samsung Monitor"));
            Assert.That(result.Product.Price,
                Is.EqualTo(1200m));
            Assert.That(result.Product.IsDeleted, Is.False);

            Assert.That(result.Category.Id, Is.EqualTo(1));
            Assert.That(result.Category.CategoryName,
                Is.EqualTo("Electronics"));

            Assert.That(result.Attributes, Has.Count.EqualTo(1));
        });

        var attribute = result.Attributes.Single();

        Assert.Multiple(() =>
        {
            Assert.That(attribute.AttributeId, Is.EqualTo(1));
            Assert.That(attribute.AttributeName,
                Is.EqualTo("Brand"));
            Assert.That(attribute.AttributeType,
                Is.EqualTo(1));

            Assert.That(attribute.TextValue,
                Is.EqualTo("Samsung"));

            Assert.That(attribute.NumberValue, Is.Null);
            Assert.That(attribute.DateValue, Is.Null);
            Assert.That(attribute.BooleanValue, Is.Null);
        });
    }


    [Test]
    public void GetProductDetails_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service.GetProductDetails(int.MaxValue);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetProductDetails_WhenProductIsDeleted_ShouldReturnDeletedProduct()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        UnitOfWork.ProductRepository.Delete(1);

        // Act
        var result = service.GetProductDetails(1);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(result!.Product.Id, Is.EqualTo(1));
            Assert.That(result.Product.IsDeleted, Is.True);
            Assert.That(result.Category, Is.Not.Null);
            Assert.That(result.Attributes, Is.Not.Null);
        });
    }


    [Test]
    public void GetProductDetails_WhenAttributeHasNoValue_ShouldReturnAttributeWithNullValues()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        UnitOfWork.ProductRepository
            .DeleteAttributeValue(1, 1);

        // Act
        var result = service.GetProductDetails(1);

        // Assert
        Assert.That(result, Is.Not.Null);

        var attribute = result!.Attributes
            .Single(x => x.AttributeId == 1);

        Assert.Multiple(() =>
        {
            Assert.That(attribute.AttributeId, Is.EqualTo(1));
            Assert.That(attribute.AttributeName,
                Is.EqualTo("Brand"));

            Assert.That(attribute.TextValue, Is.Null);
            Assert.That(attribute.NumberValue, Is.Null);
            Assert.That(attribute.DateValue, Is.Null);
            Assert.That(attribute.BooleanValue, Is.Null);
        });
    }


    [Test]
    public void GetProductDetails_WhenAttributeIsDeleted_ShouldReturnProductWithoutDeletedAttribute()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        UnitOfWork.AttributeRepository.Delete(1);

        // Act
        var result = service.GetProductDetails(1);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.That(
            result!.Attributes.Any(x => x.AttributeId == 1),
            Is.False);
    }

    [Test]
    public void GetAllProducts_WhenParameterIsNotPassed_ShouldReturnOnlyActiveProducts()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var products = service
            .GetAllProducts()
            .ToList();

        // Assert
        Assert.That(products, Is.Not.Empty);
        Assert.That(products.All(product => !product.IsDeleted), Is.True);
    }

    // =========================================================
    // GetProductByName
    // =========================================================

    [Test]
    public void GetProductByName_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var service = new ProductService(UnitOfWork, Log.Logger);

        // Act
        var result = service.GetProductByName("Samsung Monitor");

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.ProductName, Is.EqualTo("Samsung Monitor"));
            Assert.That(result.CategoryId, Is.EqualTo(1));
            Assert.That(result.Price, Is.EqualTo(1200m));
            Assert.That(result.IsDeleted, Is.False);
        });
    }
}