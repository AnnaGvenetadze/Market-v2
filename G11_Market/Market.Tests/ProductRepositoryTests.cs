//using Dapper;
//using Market.DTO;
//using Market.Tests.Helpers;
//using Microsoft.Data.SqlClient;

//namespace Market.Tests;

//public sealed class ProductRepositoryTests : BaseRepositoryTests
//{
//    [Test]
//    public void Insert_ShouldReturnIdAndCreateActiveProduct()
//    {
//        // Arrange
//        var product = new ProductDTO
//        {
//            CategoryId = 1,
//            ProductName = "Inserted Product".AddGuid(),
//            Price = 149.99m
//        };

//        // Act
//        var insertedId = UnitOfWork.ProductRepository.Insert(product);

//        // Assert
//        var storedProduct = GetProductDirect(insertedId);

//        Assert.Multiple(() =>
//        {
//            Assert.That(insertedId, Is.GreaterThan(0));
//            Assert.That(storedProduct.CategoryId, Is.EqualTo(product.CategoryId));
//            Assert.That(storedProduct.ProductName, Is.EqualTo(product.ProductName));
//            Assert.That(storedProduct.Price, Is.EqualTo(product.Price));
//            Assert.That(storedProduct.IsDeleted, Is.False);
//        });
//    }

//    [Test]
//    public void Insert_WithUnknownCategory_ShouldThrowSqlException()
//    {
//        // Arrange
//        var product = new ProductDTO
//        {
//            CategoryId = int.MaxValue,
//            ProductName = "Unknown Category Product".AddGuid(),
//            Price = 100m
//        };

//        // Act
//        var exception = Assert.Throws<SqlException>(
//            () => UnitOfWork.ProductRepository.Insert(product));

//        // Assert
//        Assert.That(exception, Is.Not.Null);
//        Assert.That(exception!.Number, Is.EqualTo(50000));
//        Assert.That(exception.Message, Does.Contain("Active category not found"));
//    }

//    [Test]
//    public void Insert_WithDuplicateName_ShouldThrowUniqueViolation()
//    {
//        // Arrange
//        var duplicateName = "Duplicate Product".AddGuid();
//        InsertProductDirect(duplicateName);

//        var duplicateProduct = new ProductDTO
//        {
//            CategoryId = 1,
//            ProductName = duplicateName,
//            Price = 200m
//        };

//        // Act
//        var exception = Assert.Throws<SqlException>(
//            () => UnitOfWork.ProductRepository.Insert(duplicateProduct));

//        // Assert
//        Assert.That(exception, Is.Not.Null);
//        Assert.That(exception!.Number, Is.AnyOf(2601, 2627));
//    }

//    [Test]
//    public void Update_ShouldChangeProductFields()
//    {
//        // Arrange
//        var productId = InsertProductDirect(
//            "Product Before Update".AddGuid(),
//            categoryId: 1,
//            price: 100m);

//        var updatedProduct = new ProductDTO
//        {
//            Id = productId,
//            CategoryId = 2,
//            ProductName = "Product After Update".AddGuid(),
//            Price = 275.50m
//        };

//        // Act
//        UnitOfWork.ProductRepository.Update(updatedProduct);

//        // Assert
//        var storedProduct = GetProductDirect(productId);

//        Assert.Multiple(() =>
//        {
//            Assert.That(storedProduct.CategoryId, Is.EqualTo(updatedProduct.CategoryId));
//            Assert.That(storedProduct.ProductName, Is.EqualTo(updatedProduct.ProductName));
//            Assert.That(storedProduct.Price, Is.EqualTo(updatedProduct.Price));
//            Assert.That(storedProduct.IsDeleted, Is.False);
//            Assert.That(storedProduct.UpdatedDate, Is.Not.Null);
//        });
//    }

//    [Test]
//    public void Delete_ShouldSoftDeleteProduct()
//    {
//        // Arrange
//        var productId = InsertProductDirect("Product To Delete".AddGuid());

//        // Act
//        UnitOfWork.ProductRepository.Delete(productId);

//        // Assert
//        var storedProduct = GetProductDirect(productId);

//        Assert.Multiple(() =>
//        {
//            Assert.That(storedProduct.Id, Is.EqualTo(productId));
//            Assert.That(storedProduct.IsDeleted, Is.True);
//            Assert.That(storedProduct.UpdatedDate, Is.Not.Null);
//        });
//    }

//    [Test]
//    public void GetById_ShouldReturnActiveProduct()
//    {
//        // Arrange
//        var productName = "Product By Id".AddGuid();
//        var productId = InsertProductDirect(productName, price: 125m);

//        // Act
//        var result = UnitOfWork.ProductRepository.GetById(productId);

//        // Assert
//        Assert.That(result, Is.Not.Null);
//        Assert.Multiple(() =>
//        {
//            Assert.That(result!.Id, Is.EqualTo(productId));
//            Assert.That(result.ProductName, Is.EqualTo(productName));
//            Assert.That(result.CategoryId, Is.EqualTo(1));
//            Assert.That(result.Price, Is.EqualTo(125m));
//            Assert.That(result.IsDeleted, Is.False);
//        });
//    }

//    [Test]
//    public void GetAll_ShouldReturnOnlyActiveProducts()
//    {
//        // Arrange
//        var activeId = InsertProductDirect("Active Product".AddGuid());
//        var deletedId = InsertProductDirect(
//            "Deleted Product".AddGuid(),
//            isDeleted: true);

//        // Act
//        var results = UnitOfWork.ProductRepository.GetAll().ToList();

//        // Assert
//        Assert.Multiple(() =>
//        {
//            Assert.That(results.Any(product => product.Id == activeId), Is.True);
//            Assert.That(results.Any(product => product.Id == deletedId), Is.False);
//            Assert.That(results.All(product => product.IsDeleted == false), Is.True);
//        });
//    }

//    [Test]
//    public void GetByName_ShouldReturnMatchingActiveProduct()
//    {
//        // Arrange
//        var productName = "Find Product By Name".AddGuid();
//        var productId = InsertProductDirect(productName, price: 175m);

//        // Act
//        var result = UnitOfWork.ProductRepository.GetByName(productName);

//        // Assert
//        Assert.That(result, Is.Not.Null);
//        Assert.Multiple(() =>
//        {
//            Assert.That(result!.Id, Is.EqualTo(productId));
//            Assert.That(result.ProductName, Is.EqualTo(productName));
//            Assert.That(result.IsDeleted, Is.False);
//        });
//    }

//    [Test]
//    public void GetByName_WhenMissing_ShouldReturnNull()
//    {
//        // Arrange
//        var missingName = "Missing Product".AddGuid();

//        // Act
//        var result = UnitOfWork.ProductRepository.GetByName(missingName);

//        // Assert
//        Assert.That(result, Is.Null);
//    }

//    [Test]
//    public void GetByName_WhenWhitespace_ShouldThrowArgumentException()
//    {
//        // Arrange
//        const string whitespaceName = "   ";

//        // Act and Assert
//        Assert.Throws<ArgumentException>(
//            () => UnitOfWork.ProductRepository.GetByName(whitespaceName));
//    }

//    [Test]
//    public void GetByName_ShouldExcludeDeletedProduct()
//    {
//        // Arrange
//        var productName = "Deleted Named Product".AddGuid();
//        InsertProductDirect(productName, isDeleted: true);

//        // Act
//        var result = UnitOfWork.ProductRepository.GetByName(productName);

//        // Assert
//        Assert.That(result, Is.Null);
//    }

//    [Test]
//    public void GetByCategoryId_ShouldReturnOnlyActiveProductsInCategory()
//    {
//        // Arrange
//        var activeCategoryOneId = InsertProductDirect(
//            "Active Category One Product".AddGuid(),
//            categoryId: 1);

//        var deletedCategoryOneId = InsertProductDirect(
//            "Deleted Category One Product".AddGuid(),
//            categoryId: 1,
//            isDeleted: true);

//        var activeCategoryTwoId = InsertProductDirect(
//            "Active Category Two Product".AddGuid(),
//            categoryId: 2);

//        // Act
//        var results = UnitOfWork.ProductRepository.GetByCategoryId(1).ToList();

//        // Assert
//        Assert.Multiple(() =>
//        {
//            Assert.That(results.Any(product => product.Id == activeCategoryOneId), Is.True);
//            Assert.That(results.Any(product => product.Id == deletedCategoryOneId), Is.False);
//            Assert.That(results.Any(product => product.Id == activeCategoryTwoId), Is.False);
//            Assert.That(
//                results.All(product => product.CategoryId == 1 && product.IsDeleted == false),
//                Is.True);
//        });
//    }

//    [Test]
//    public void GetByCategoryId_WhenZero_ShouldThrowArgumentOutOfRangeException()
//    {
//        // Arrange
//        const int invalidCategoryId = 0;

//        // Act and Assert
//        Assert.Throws<ArgumentOutOfRangeException>(
//            () => UnitOfWork.ProductRepository.GetByCategoryId(invalidCategoryId));
//    }

//    [Test]
//    public void GetByPriceRange_ShouldIncludeBothBoundaries()
//    {
//        // Arrange
//        var minimumBoundaryId = InsertProductDirect(
//            "Minimum Boundary Product".AddGuid(),
//            price: 100m);

//        var middleId = InsertProductDirect(
//            "Middle Price Product".AddGuid(),
//            price: 150m);

//        var maximumBoundaryId = InsertProductDirect(
//            "Maximum Boundary Product".AddGuid(),
//            price: 200m);

//        var outsideRangeId = InsertProductDirect(
//            "Outside Range Product".AddGuid(),
//            price: 250m);

//        var deletedInsideRangeId = InsertProductDirect(
//            "Deleted Inside Range Product".AddGuid(),
//            price: 150m,
//            isDeleted: true);

//        // Act
//        var results = UnitOfWork.ProductRepository.GetByPriceRange(100m, 200m).ToList();

//        // Assert
//        Assert.Multiple(() =>
//        {
//            Assert.That(results.Any(product => product.Id == minimumBoundaryId), Is.True);
//            Assert.That(results.Any(product => product.Id == middleId), Is.True);
//            Assert.That(results.Any(product => product.Id == maximumBoundaryId), Is.True);
//            Assert.That(results.Any(product => product.Id == outsideRangeId), Is.False);
//            Assert.That(results.Any(product => product.Id == deletedInsideRangeId), Is.False);
//            Assert.That(
//                results.All(product =>
//                    product.Price >= 100m &&
//                    product.Price <= 200m &&
//                    product.IsDeleted == false),
//                Is.True);
//        });
//    }

//    [Test]
//    public void GetByPriceRange_WhenMinimumNegative_ShouldThrowArgumentOutOfRangeException()
//    {
//        // Arrange
//        const decimal negativeMinimum = -0.01m;
//        const decimal maximum = 100m;

//        // Act and Assert
//        Assert.Throws<ArgumentOutOfRangeException>(
//            () => UnitOfWork.ProductRepository.GetByPriceRange(negativeMinimum, maximum));
//    }

//    [Test]
//    public void GetByPriceRange_WhenMaximumBelowMinimum_ShouldThrowArgumentException()
//    {
//        // Arrange
//        const decimal minimum = 200m;
//        const decimal maximum = 100m;

//        // Act and Assert
//        Assert.Throws<ArgumentException>(
//            () => UnitOfWork.ProductRepository.GetByPriceRange(minimum, maximum));
//    }

//    private int InsertProductDirect(
//        string productName,
//        int categoryId = 1,
//        decimal price = 100m,
//        bool isDeleted = false)
//    {
//        return Connection.ExecuteScalar<int>(
//            """
//            INSERT INTO dbo.Products
//            (
//                CategoryId,
//                ProductName,
//                Price,
//                IsDeleted
//            )
//            OUTPUT INSERTED.Id
//            VALUES
//            (
//                @CategoryId,
//                @ProductName,
//                @Price,
//                @IsDeleted
//            );
//            """,
//            new
//            {
//                CategoryId = categoryId,
//                ProductName = productName,
//                Price = price,
//                IsDeleted = isDeleted
//            });
//    }

//    private ProductDTO GetProductDirect(int productId)
//    {
//        return Connection.QuerySingle<ProductDTO>(
//            """
//            SELECT
//                Id,
//                CategoryId,
//                ProductName,
//                Price,
//                IsDeleted,
//                CreatedDate,
//                UpdatedDate
//            FROM dbo.Products
//            WHERE Id = @Id;
//            """,
//            new { Id = productId });
//    }

//    [Test]
//    public void GetAttributeValues_ShouldReturnCorrectValues()
//    {
//        // Act
//        var values = UnitOfWork.ProductRepository
//            .GetAttributeValues(1)
//            .ToList();

//        // Assert
//        Assert.That(values, Has.Count.EqualTo(1));

//        var value = values.Single();

//        Assert.Multiple(() =>
//        {
//            Assert.That(value.ProductId, Is.EqualTo(1));
//            Assert.That(value.AttributeId, Is.EqualTo(1));
//            Assert.That(value.TextValue, Is.EqualTo("Samsung"));
//            Assert.That(value.NumberValue, Is.Null);
//            Assert.That(value.DateValue, Is.Null);
//            Assert.That(value.BooleanValue, Is.Null);
//        });
//    }

//    [Test]
//    public void InsertAttributeValue_ShouldAddValue()
//    {
//        // Arrange
//        // Seed-ში ეს წყვილი უკვე არსებობს, ამიტომ ჯერ ვათავისუფლებთ.
//        UnitOfWork.ProductRepository.DeleteAttributeValue(1, 1);

//        var value = new ProductAttributeValueDTO
//        {
//            ProductId = 1,
//            AttributeId = 1,
//            TextValue = "LG"
//        };

//        // Act
//        UnitOfWork.ProductRepository.InsertAttributeValue(value);

//        // Assert
//        var values = UnitOfWork.ProductRepository
//            .GetAttributeValues(1)
//            .ToList();

//        Assert.That(values, Has.Count.EqualTo(1));

//        var addedValue = values.Single();

//        Assert.Multiple(() =>
//        {
//            Assert.That(addedValue.ProductId, Is.EqualTo(1));
//            Assert.That(addedValue.AttributeId, Is.EqualTo(1));
//            Assert.That(addedValue.TextValue, Is.EqualTo("LG"));
//            Assert.That(addedValue.NumberValue, Is.Null);
//            Assert.That(addedValue.DateValue, Is.Null);
//            Assert.That(addedValue.BooleanValue, Is.Null);
//        });
//    }

//    [Test]
//    public void UpdateAttributeValue_ShouldUpdateValue()
//    {
//        // Arrange
//        // 1. Seed Category
//        var category = new CategoryDTO
//        {
//            CategoryName = "Test Category " + Guid.NewGuid(),
//            Description = "Category for attribute testing"
//        };
//        int categoryId = UnitOfWork.CategoryRepository.Insert(category);

//        // 2. Seed Attribute (AttributeType = 2 for Numeric)
//        var attribute = new AttributeDTO
//        {
//            AttributeName = "Test Weight " + Guid.NewGuid(),
//            AttributeType = 2 // 2 = Numeric
//        };
//        int attributeId = UnitOfWork.AttributeRepository.Insert(attribute);

//        // 3. Link Attribute to Category (Satisfies active category attribute assignment requirement)
//        var categoryAttribute = new CategoryAttributeDTO
//        {
//            CategoryId = categoryId,
//            AttributeId = attributeId,
//            OrderPosition = 1
//        };
//        UnitOfWork.CategoryRepository.AssignAttribute(categoryAttribute);

//        // 4. Seed Product linked to Category
//        var product = new ProductDTO
//        {
//            CategoryId = categoryId,
//            ProductName = "Test Product " + Guid.NewGuid(),
//            Price = 29.99m
//        };
//        int productId = UnitOfWork.ProductRepository.Insert(product);

//        // 5. Seed initial attribute value (Satisfies UPDATE existence requirement)
//        var initialValue = new ProductAttributeValueDTO
//        {
//            ProductId = productId,
//            AttributeId = attributeId,
//            NumberValue = 1.00m
//        };
//        UnitOfWork.ProductRepository.InsertAttributeValue(initialValue);

//        // DTO to update
//        var valueToUpdate = new ProductAttributeValueDTO
//        {
//            ProductId = productId,
//            AttributeId = attributeId,
//            NumberValue = 3.75m
//        };

//        // Act
//        UnitOfWork.ProductRepository.UpdateAttributeValue(valueToUpdate);

//        // Assert
//        var values = UnitOfWork.ProductRepository
//            .GetAttributeValues(productId)
//            .ToList();

//        Assert.That(values, Has.Count.EqualTo(1));

//        var updatedValue = values.Single(v => v.AttributeId == attributeId);

//        Assert.Multiple(() =>
//        {
//            Assert.That(updatedValue.ProductId, Is.EqualTo(productId));
//            Assert.That(updatedValue.AttributeId, Is.EqualTo(attributeId));
//            Assert.That(updatedValue.NumberValue, Is.EqualTo(3.75m));
//            Assert.That(updatedValue.TextValue, Is.Null);
//            Assert.That(updatedValue.DateValue, Is.Null);
//            Assert.That(updatedValue.BooleanValue, Is.Null);
//        });
//    }

//    [Test]
//    public void DeleteAttributeValue_ShouldDeleteValue()
//    {
//        // Act
//        UnitOfWork.ProductRepository.DeleteAttributeValue(2, 2);

//        // Assert
//        var deletedProductValues = UnitOfWork.ProductRepository
//            .GetAttributeValues(2)
//            .ToList();

//        var otherProductValues = UnitOfWork.ProductRepository
//            .GetAttributeValues(1)
//            .ToList();

//        Assert.Multiple(() =>
//        {
//            Assert.That(deletedProductValues, Is.Empty);
//            Assert.That(otherProductValues, Has.Count.EqualTo(1));
//        });

//        Assert.That(
//            otherProductValues.Single().TextValue,
//            Is.EqualTo("Samsung"));
//    }

//    [Test]
//    public void Restore_ShouldRestoreDeletedProduct()
//    {
//        // Arrange
//        UnitOfWork.ProductRepository.Delete(DeleteTestId);

//        // Act
//        UnitOfWork.ProductRepository.Restore(DeleteTestId);

//        // Assert
//        var product = UnitOfWork.ProductRepository.GetById(DeleteTestId);

//        Assert.That(product, Is.Not.Null);
//        Assert.That(product!.IsDeleted, Is.False);
//    }

//    [Test]
//    public void Restore_WhenProductDoesNotExist_ShouldThrow()
//    {
//        Assert.Throws<SqlException>(() =>
//            UnitOfWork.ProductRepository.Restore(int.MaxValue));
//    }

//    [Test]
//    public void Restore_WhenProductIsNotDeleted_ShouldThrow()
//    {
//        Assert.Throws<SqlException>(() =>
//            UnitOfWork.ProductRepository.Restore(1));
//    }
//}
