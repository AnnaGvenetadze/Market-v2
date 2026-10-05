using Market.DTO;
using Market.Services;
using Market.Services.Interfaces.Services;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Repositories;
using Moq;
using Serilog;

namespace Market.Tests.Services;

[TestFixture]
public class ProductServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private Mock<ICategoryRepository> _categoryRepoMock = null!;
    private Mock<ILogger> _loggerMock = null!;
    private ProductService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepoMock = new Mock<IProductRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _loggerMock = new Mock<ILogger>();

        _unitOfWorkMock.Setup(u => u.ProductRepository).Returns(_productRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CategoryRepository).Returns(_categoryRepoMock.Object);

        _sut = new ProductService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    #region Constructor Tests

    [Test]
    public void Constructor_NullUnitOfWork_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ProductService(null!, _loggerMock.Object));
    }

    [Test]
    public void Constructor_NullLogger_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ProductService(_unitOfWorkMock.Object, null!));
    }

    #endregion

    #region CreateProduct Tests

    [Test]
    public void CreateProduct_NullProduct_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.CreateProduct(null!));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void CreateProduct_InvalidCategoryId_ShouldThrowArgumentOutOfRangeException(int categoryId)
    {
        var product = new ProductDTO { CategoryId = categoryId, ProductName = "Valid Product" };

        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.CreateProduct(product));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void CreateProduct_InvalidProductName_ShouldThrowArgumentException(string? productName)
    {
        var product = new ProductDTO { CategoryId = 1, ProductName = productName! };

        Assert.Throws<ArgumentException>(() => _sut.CreateProduct(product));
    }

    [Test]
    public void CreateProduct_CategoryDoesNotExist_ShouldThrowInvalidOperationExceptionAndLogError()
    {
        var product = new ProductDTO { CategoryId = 99, ProductName = "Monitor" };
        _categoryRepoMock.Setup(r => r.GetById(99)).Returns((CategoryDTO?)null);

        var ex = Assert.Throws<InvalidOperationException>(() => _sut.CreateProduct(product));

        Assert.That(ex!.Message, Is.EqualTo("Category with ID 99 does not exist."));
        _loggerMock.Verify(x => x.Error("Failed to create product: CategoryId {CategoryId} does not exist", 99), Times.Once);
    }

    [Test]
    public void CreateProduct_ValidProduct_ShouldCommitTransactionAndReturnId()
    {
        // Arrange
        var product = new ProductDTO { CategoryId = 1, ProductName = "Gaming Mouse" };
        var category = new CategoryDTO { Id = 1, CategoryName = "Peripherals" };

        _categoryRepoMock.Setup(r => r.GetById(1)).Returns(category);
        _productRepoMock.Setup(r => r.Insert(product)).Returns(101);

        // Act
        var result = _sut.CreateProduct(product);

        // Assert
        Assert.That(result, Is.EqualTo(101));
        _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
        _productRepoMock.Verify(r => r.Insert(product), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Never);
    }

    [Test]
    public void CreateProduct_WithAttributeValues_ShouldInsertAttributesAndCommit()
    {
        // Arrange
        var product = new ProductDTO { CategoryId = 1, ProductName = "Keyboard" };
        var category = new CategoryDTO { Id = 1, CategoryName = "Peripherals" };
        var attributes = new List<ProductAttributeValueDTO>
        {
            new() { AttributeId = 1, TextValue = "Mechanical" },
            new() { AttributeId = 2, BooleanValue = true }
        };

        _categoryRepoMock.Setup(r => r.GetById(1)).Returns(category);
        _productRepoMock.Setup(r => r.Insert(product)).Returns(50);

        // Act
        var result = _sut.CreateProduct(product, attributes);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(50));
            Assert.That(attributes.All(a => a.ProductId == 50), Is.True);
        });
        _productRepoMock.Verify(r => r.InsertAttributeValue(It.IsAny<ProductAttributeValueDTO>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Test]
    public void CreateProduct_OnException_ShouldRollbackAndLogError()
    {
        // Arrange
        var product = new ProductDTO { CategoryId = 1, ProductName = "Headset" };
        _categoryRepoMock.Setup(r => r.GetById(1)).Returns(new CategoryDTO { Id = 1 });
        _productRepoMock.Setup(r => r.Insert(product)).Throws(new InvalidOperationException("DB Failure"));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _sut.CreateProduct(product));

        _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), "Failed to create product {ProductName} in category {CategoryId}", "Headset", 1), Times.Once);
    }

    #endregion

    #region UpdateProduct Tests

    [Test]
    public void UpdateProduct_NullProduct_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.UpdateProduct(null!));
    }

    [TestCase(0, 1, "Name")]
    [TestCase(-1, 1, "Name")]
    [TestCase(1, 0, "Name")]
    [TestCase(1, -1, "Name")]
    [TestCase(1, 1, "")]
    [TestCase(1, 1, "   ")]
    public void UpdateProduct_InvalidArguments_ShouldThrowException(int id, int categoryId, string name)
    {
        var product = new ProductDTO { Id = id, CategoryId = categoryId, ProductName = name };

        Assert.That(() => _sut.UpdateProduct(product), Throws.Exception);
    }

    [Test]
    public void UpdateProduct_ProductDoesNotExist_ShouldThrowInvalidOperationExceptionAndLogWarning()
    {
        var product = new ProductDTO { Id = 10, CategoryId = 1, ProductName = "Updated Item" };
        _productRepoMock.Setup(r => r.GetById(10)).Returns((ProductDTO?)null);

        var ex = Assert.Throws<InvalidOperationException>(() => _sut.UpdateProduct(product));

        Assert.That(ex!.Message, Is.EqualTo("Product with ID 10 does not exist."));
        _loggerMock.Verify(x => x.Warning("Update failed: ProductId {ProductId} does not exist", 10), Times.Once);
    }

    [Test]
    public void UpdateProduct_SyncsAttributes_UpdatesExistingAndInsertsNew()
    {
        // Arrange
        var product = new ProductDTO { Id = 10, CategoryId = 1, ProductName = "Updated Mouse" };
        var existingAttributes = new List<ProductAttributeValueDTO>
        {
            new() { ProductId = 10, AttributeId = 1, TextValue = "Black" }
        };
        var incomingAttributes = new List<ProductAttributeValueDTO>
        {
            new() { AttributeId = 1, TextValue = "White" },   // Existing -> Update
            new() { AttributeId = 2, BooleanValue = true }    // New -> Insert
        };

        _productRepoMock.Setup(r => r.GetById(10)).Returns(product);
        _productRepoMock.Setup(r => r.GetAttributeValues(10)).Returns(existingAttributes);

        // Act
        var result = _sut.UpdateProduct(product, incomingAttributes);

        // Assert
        Assert.That(result, Is.EqualTo(10));
        _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
        _productRepoMock.Verify(r => r.Update(product), Times.Once);
        _productRepoMock.Verify(r => r.UpdateAttributeValue(It.Is<ProductAttributeValueDTO>(a => a.AttributeId == 1 && a.TextValue == "White")), Times.Once);
        _productRepoMock.Verify(r => r.InsertAttributeValue(It.Is<ProductAttributeValueDTO>(a => a.AttributeId == 2 && a.BooleanValue == true)), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Test]
    public void UpdateProduct_OnException_ShouldRollbackTransaction()
    {
        var product = new ProductDTO { Id = 10, CategoryId = 1, ProductName = "Mouse" };
        _productRepoMock.Setup(r => r.GetById(10)).Returns(product);
        _productRepoMock.Setup(r => r.Update(product)).Throws(new Exception("Write Error"));

        Assert.Throws<Exception>(() => _sut.UpdateProduct(product));

        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), "Failed to update product {ProductId}", 10), Times.Once);
    }

    #endregion

    #region Delete & Restore Tests

    [TestCase(0)]
    [TestCase(-1)]
    public void DeleteProduct_InvalidId_ShouldThrowArgumentOutOfRangeException(int productId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.DeleteProduct(productId));
    }

    [Test]
    public void DeleteProduct_ValidId_ShouldCallRepositoryDelete()
    {
        _sut.DeleteProduct(5);

        _productRepoMock.Verify(r => r.Delete(5), Times.Once);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void RestoreProduct_InvalidId_ShouldThrowArgumentOutOfRangeException(int productId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.RestoreProduct(productId));
    }

    [Test]
    public void RestoreProduct_ValidId_ShouldCallRepositoryRestore()
    {
        _sut.RestoreProduct(5);

        _productRepoMock.Verify(r => r.Restore(5), Times.Once);
    }

    #endregion

    #region Query Methods Tests

    [TestCase(0)]
    [TestCase(-1)]
    public void GetProductById_InvalidId_ShouldThrowArgumentOutOfRangeException(int productId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.GetProductById(productId));
    }

    [Test]
    public void GetProductById_WhenNotFound_ShouldLogWarningAndReturnNull()
    {
        _productRepoMock.Setup(r => r.GetById(42)).Returns((ProductDTO?)null);

        var result = _sut.GetProductById(42);

        Assert.That(result, Is.Null);
        _loggerMock.Verify(x => x.Warning("ProductId {ProductId} was not found", 42), Times.Once);
    }

    [Test]
    public void GetProductById_WhenFound_ShouldReturnProduct()
    {
        var product = new ProductDTO { Id = 42, ProductName = "Chair" };
        _productRepoMock.Setup(r => r.GetById(42)).Returns(product);

        var result = _sut.GetProductById(42);

        Assert.That(result, Is.EqualTo(product));
    }

    [Test]
    public void GetAllProducts_ShouldReturnRepositoryCollection()
    {
        var products = new List<ProductDTO> { new() { Id = 1 }, new() { Id = 2 } };
        _productRepoMock.Setup(r => r.GetAll()).Returns(products);

        var result = _sut.GetAllProducts();

        Assert.That(result, Is.EqualTo(products));
        _productRepoMock.Verify(r => r.GetAll(), Times.Once);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetProductsByCategoryId_InvalidCategoryId_ShouldThrowArgumentOutOfRangeException(int categoryId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.GetProductsByCategoryId(categoryId));
    }

    [Test]
    public void GetProductsByCategoryId_ValidCategoryId_ShouldReturnProducts()
    {
        var products = new List<ProductDTO> { new() { Id = 1, CategoryId = 5 } };
        _productRepoMock.Setup(r => r.GetByCategoryId(5)).Returns(products);

        var result = _sut.GetProductsByCategoryId(5);

        Assert.That(result, Is.EqualTo(products));
    }

    [Test]
    public void GetProductsByPriceRange_WhenMinNegative_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.GetProductsByPriceRange(-1m, 100m));
    }

    [Test]
    public void GetProductsByPriceRange_WhenMaxLessThanMin_ShouldThrowArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => _sut.GetProductsByPriceRange(100m, 50m));

        Assert.That(ex!.Message, Does.Contain("Maximum price must be greater than or equal to minimum price"));
    }

    [Test]
    public void GetProductsByPriceRange_ValidRange_ShouldReturnProducts()
    {
        var products = new List<ProductDTO> { new() { Id = 1, Price = 75m } };
        _productRepoMock.Setup(r => r.GetByPriceRange(50m, 100m)).Returns(products);

        var result = _sut.GetProductsByPriceRange(50m, 100m);

        Assert.That(result, Is.EqualTo(products));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetProductAttributeValues_InvalidId_ShouldThrowArgumentOutOfRangeException(int productId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.GetProductAttributeValues(productId));
    }

    [Test]
    public void GetProductAttributeValues_ValidId_ShouldReturnValues()
    {
        var values = new List<ProductAttributeValueDTO> { new() { ProductId = 1, AttributeId = 2 } };
        _productRepoMock.Setup(r => r.GetAttributeValues(1)).Returns(values);

        var result = _sut.GetProductAttributeValues(1);

        Assert.That(result, Is.EqualTo(values));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetProductDetails_InvalidId_ShouldThrowArgumentOutOfRangeException(int productId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.GetProductDetails(productId));
    }

    [Test]
    public void GetProductDetails_WhenMissing_ShouldLogWarningAndReturnNull()
    {
        _productRepoMock.Setup(r => r.GetProductDetails(10)).Returns((ProductDetailsDTO?)null);

        var result = _sut.GetProductDetails(10);

        Assert.That(result, Is.Null);
        _loggerMock.Verify(x => x.Warning("Product with ID {ProductId} was not found.", 10), Times.Once);
    }

    [Test]
    public void GetProductDetails_WhenDeleted_ShouldLogInformationAndReturnDetails()
    {
        var details = new ProductDetailsDTO
        {
            Product = new ProductDTO { Id = 10, IsDeleted = true, ProductName = "Inactive Item" },
            Category = new CategoryDTO { Id = 1, CategoryName = "Category" },
            Attributes = new List<ProductDetailAttributeDTO>()
        };
        _productRepoMock.Setup(r => r.GetProductDetails(10)).Returns(details);

        var result = _sut.GetProductDetails(10);

        Assert.That(result, Is.EqualTo(details));
        _loggerMock.Verify(x => x.Information("Product with ID {ProductId} is inactive.", 10), Times.Once);
    }

    #endregion

    #region Direct Attribute Operations Tests

    [Test]
    public void AddProductAttributeValue_NullAttribute_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.AddProductAttributeValue(null!));
    }

    [TestCase(0, 1)]
    [TestCase(-1, 1)]
    [TestCase(1, 0)]
    [TestCase(1, -1)]
    public void AddProductAttributeValue_InvalidIds_ShouldThrowArgumentOutOfRangeException(int productId, int attributeId)
    {
        var attr = new ProductAttributeValueDTO { ProductId = productId, AttributeId = attributeId };

        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.AddProductAttributeValue(attr));
    }

    [Test]
    public void AddProductAttributeValue_ValidAttribute_ShouldCallInsert()
    {
        var attr = new ProductAttributeValueDTO { ProductId = 1, AttributeId = 2 };

        _sut.AddProductAttributeValue(attr);

        _productRepoMock.Verify(r => r.InsertAttributeValue(attr), Times.Once);
    }

    [Test]
    public void UpdateProductAttributeValue_NullAttribute_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.UpdateProductAttributeValue(null!));
    }

    [TestCase(0, 1)]
    [TestCase(-1, 1)]
    [TestCase(1, 0)]
    [TestCase(1, -1)]
    public void UpdateProductAttributeValue_InvalidIds_ShouldThrowArgumentOutOfRangeException(int productId, int attributeId)
    {
        var attr = new ProductAttributeValueDTO { ProductId = productId, AttributeId = attributeId };

        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.UpdateProductAttributeValue(attr));
    }

    [Test]
    public void UpdateProductAttributeValue_ValidAttribute_ShouldCallUpdate()
    {
        var attr = new ProductAttributeValueDTO { ProductId = 1, AttributeId = 2 };

        _sut.UpdateProductAttributeValue(attr);

        _productRepoMock.Verify(r => r.UpdateAttributeValue(attr), Times.Once);
    }

    [TestCase(0, 1)]
    [TestCase(-1, 1)]
    [TestCase(1, 0)]
    [TestCase(1, -1)]
    public void DeleteProductAttributeValue_InvalidIds_ShouldThrowArgumentOutOfRangeException(int productId, int attributeId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.DeleteProductAttributeValue(productId, attributeId));
    }

    [Test]
    public void DeleteProductAttributeValue_ValidIds_ShouldCallDelete()
    {
        _sut.DeleteProductAttributeValue(1, 2);

        _productRepoMock.Verify(r => r.DeleteAttributeValue(1, 2), Times.Once);
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