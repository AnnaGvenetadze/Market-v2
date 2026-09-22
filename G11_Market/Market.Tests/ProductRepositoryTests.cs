using Dapper;
using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public sealed class ProductRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void Insert_ShouldReturnIdAndCreateActiveProduct()
    {
        // Arrange
        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "Inserted Product".AddGuid(),
            Price = 149.99m
        };

        // Act
        var insertedId = UnitOfWork.ProductRepository.AssignAttribute(product);

        // Assert
        var storedProduct = GetProductDirect(insertedId);

        Assert.Multiple(() =>
        {
            Assert.That(insertedId, Is.GreaterThan(0));
            Assert.That(storedProduct.CategoryId, Is.EqualTo(product.CategoryId));
            Assert.That(storedProduct.ProductName, Is.EqualTo(product.ProductName));
            Assert.That(storedProduct.Price, Is.EqualTo(product.Price));
            Assert.That(storedProduct.IsDeleted, Is.False);
        });
    }

    [Test]
    public void Insert_WithUnknownCategory_ShouldThrowForeignKeyViolation()
    {
        // Arrange
        var product = new ProductDTO
        {
            CategoryId = int.MaxValue,
            ProductName = "Unknown Category Product".AddGuid(),
            Price = 100m
        };

        // Act
        var exception = Assert.Throws<SqlException>(
            () => UnitOfWork.ProductRepository.AssignAttribute(product));

        // Assert
        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Number, Is.EqualTo(547));
    }

    [Test]
    public void Insert_WithDuplicateName_ShouldThrowUniqueViolation()
    {
        // Arrange
        var duplicateName = "Duplicate Product".AddGuid();
        InsertProductDirect(duplicateName);

        var duplicateProduct = new ProductDTO
        {
            CategoryId = 1,
            ProductName = duplicateName,
            Price = 200m
        };

        // Act
        var exception = Assert.Throws<SqlException>(
            () => UnitOfWork.ProductRepository.AssignAttribute(duplicateProduct));

        // Assert
        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Number, Is.AnyOf(2601, 2627));
    }

    [Test]
    public void Update_ShouldChangeProductFields()
    {
        // Arrange
        var productId = InsertProductDirect(
            "Product Before Update".AddGuid(),
            categoryId: 1,
            price: 100m);

        var updatedProduct = new ProductDTO
        {
            Id = productId,
            CategoryId = 2,
            ProductName = "Product After Update".AddGuid(),
            Price = 275.50m
        };

        // Act
        UnitOfWork.ProductRepository.Update(updatedProduct);

        // Assert
        var storedProduct = GetProductDirect(productId);

        Assert.Multiple(() =>
        {
            Assert.That(storedProduct.CategoryId, Is.EqualTo(updatedProduct.CategoryId));
            Assert.That(storedProduct.ProductName, Is.EqualTo(updatedProduct.ProductName));
            Assert.That(storedProduct.Price, Is.EqualTo(updatedProduct.Price));
            Assert.That(storedProduct.IsDeleted, Is.False);
            Assert.That(storedProduct.UpdatedDate, Is.Not.Null);
        });
    }

    [Test]
    public void Delete_ShouldSoftDeleteProduct()
    {
        // Arrange
        var productId = InsertProductDirect("Product To Delete".AddGuid());

        // Act
        UnitOfWork.ProductRepository.Delete(productId);

        // Assert
        var storedProduct = GetProductDirect(productId);

        Assert.Multiple(() =>
        {
            Assert.That(storedProduct.Id, Is.EqualTo(productId));
            Assert.That(storedProduct.IsDeleted, Is.True);
            Assert.That(storedProduct.UpdatedDate, Is.Not.Null);
        });
    }

    [Test]
    public void GetById_ShouldReturnActiveProduct()
    {
        // Arrange
        var productName = "Product By Id".AddGuid();
        var productId = InsertProductDirect(productName, price: 125m);

        // Act
        var result = UnitOfWork.ProductRepository.GetById(productId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(productId));
            Assert.That(result.ProductName, Is.EqualTo(productName));
            Assert.That(result.CategoryId, Is.EqualTo(1));
            Assert.That(result.Price, Is.EqualTo(125m));
            Assert.That(result.IsDeleted, Is.False);
        });
    }

    [Test]
    public void GetAll_ShouldReturnOnlyActiveProducts()
    {
        // Arrange
        var activeId = InsertProductDirect("Active Product".AddGuid());
        var deletedId = InsertProductDirect(
            "Deleted Product".AddGuid(),
            isDeleted: true);

        // Act
        var results = UnitOfWork.ProductRepository.GetAll().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(product => product.Id == activeId), Is.True);
            Assert.That(results.Any(product => product.Id == deletedId), Is.False);
            Assert.That(results.All(product => product.IsDeleted == false), Is.True);
        });
    }

    [Test]
    public void GetByName_ShouldReturnMatchingActiveProduct()
    {
        // Arrange
        var productName = "Find Product By Name".AddGuid();
        var productId = InsertProductDirect(productName, price: 175m);

        // Act
        var result = UnitOfWork.ProductRepository.GetByName(productName);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(productId));
            Assert.That(result.ProductName, Is.EqualTo(productName));
            Assert.That(result.IsDeleted, Is.False);
        });
    }

    [Test]
    public void GetByName_WhenMissing_ShouldReturnNull()
    {
        // Arrange
        var missingName = "Missing Product".AddGuid();

        // Act
        var result = UnitOfWork.ProductRepository.GetByName(missingName);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetByName_WhenWhitespace_ShouldThrowArgumentException()
    {
        // Arrange
        const string whitespaceName = "   ";

        // Act and Assert
        Assert.Throws<ArgumentException>(
            () => UnitOfWork.ProductRepository.GetByName(whitespaceName));
    }

    [Test]
    public void GetByName_ShouldExcludeDeletedProduct()
    {
        // Arrange
        var productName = "Deleted Named Product".AddGuid();
        InsertProductDirect(productName, isDeleted: true);

        // Act
        var result = UnitOfWork.ProductRepository.GetByName(productName);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetByCategoryId_ShouldReturnOnlyActiveProductsInCategory()
    {
        // Arrange
        var activeCategoryOneId = InsertProductDirect(
            "Active Category One Product".AddGuid(),
            categoryId: 1);

        var deletedCategoryOneId = InsertProductDirect(
            "Deleted Category One Product".AddGuid(),
            categoryId: 1,
            isDeleted: true);

        var activeCategoryTwoId = InsertProductDirect(
            "Active Category Two Product".AddGuid(),
            categoryId: 2);

        // Act
        var results = UnitOfWork.ProductRepository.GetByCategoryId(1).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(product => product.Id == activeCategoryOneId), Is.True);
            Assert.That(results.Any(product => product.Id == deletedCategoryOneId), Is.False);
            Assert.That(results.Any(product => product.Id == activeCategoryTwoId), Is.False);
            Assert.That(
                results.All(product => product.CategoryId == 1 && product.IsDeleted == false),
                Is.True);
        });
    }

    [Test]
    public void GetByCategoryId_WhenZero_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int invalidCategoryId = 0;

        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => UnitOfWork.ProductRepository.GetByCategoryId(invalidCategoryId));
    }

    [Test]
    public void GetByPriceRange_ShouldIncludeBothBoundaries()
    {
        // Arrange
        var minimumBoundaryId = InsertProductDirect(
            "Minimum Boundary Product".AddGuid(),
            price: 100m);

        var middleId = InsertProductDirect(
            "Middle Price Product".AddGuid(),
            price: 150m);

        var maximumBoundaryId = InsertProductDirect(
            "Maximum Boundary Product".AddGuid(),
            price: 200m);

        var outsideRangeId = InsertProductDirect(
            "Outside Range Product".AddGuid(),
            price: 250m);

        var deletedInsideRangeId = InsertProductDirect(
            "Deleted Inside Range Product".AddGuid(),
            price: 150m,
            isDeleted: true);

        // Act
        var results = UnitOfWork.ProductRepository.GetByPriceRange(100m, 200m).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(product => product.Id == minimumBoundaryId), Is.True);
            Assert.That(results.Any(product => product.Id == middleId), Is.True);
            Assert.That(results.Any(product => product.Id == maximumBoundaryId), Is.True);
            Assert.That(results.Any(product => product.Id == outsideRangeId), Is.False);
            Assert.That(results.Any(product => product.Id == deletedInsideRangeId), Is.False);
            Assert.That(
                results.All(product =>
                    product.Price >= 100m &&
                    product.Price <= 200m &&
                    product.IsDeleted == false),
                Is.True);
        });
    }

    [Test]
    public void GetByPriceRange_WhenMinimumNegative_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const decimal negativeMinimum = -0.01m;
        const decimal maximum = 100m;

        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => UnitOfWork.ProductRepository.GetByPriceRange(negativeMinimum, maximum));
    }

    [Test]
    public void GetByPriceRange_WhenMaximumBelowMinimum_ShouldThrowArgumentException()
    {
        // Arrange
        const decimal minimum = 200m;
        const decimal maximum = 100m;

        // Act and Assert
        Assert.Throws<ArgumentException>(
            () => UnitOfWork.ProductRepository.GetByPriceRange(minimum, maximum));
    }

    private int InsertProductDirect(
        string productName,
        int categoryId = 1,
        decimal price = 100m,
        bool isDeleted = false)
    {
        return Connection.ExecuteScalar<int>(
            """
            INSERT INTO dbo.Products
            (
                CategoryId,
                ProductName,
                Price,
                IsDeleted
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @CategoryId,
                @ProductName,
                @Price,
                @IsDeleted
            );
            """,
            new
            {
                CategoryId = categoryId,
                ProductName = productName,
                Price = price,
                IsDeleted = isDeleted
            });
    }

    private ProductDTO GetProductDirect(int productId)
    {
        return Connection.QuerySingle<ProductDTO>(
            """
            SELECT
                Id,
                CategoryId,
                ProductName,
                Price,
                IsDeleted,
                CreatedDate,
                UpdatedDate
            FROM dbo.Products
            WHERE Id = @Id;
            """,
            new { Id = productId });
    }
}
