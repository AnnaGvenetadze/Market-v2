using Market.DTO;
using Market.Services;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Services;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Market.Tests;

[TestFixture]
[NonParallelizable]
public sealed class StockMovementServiceTests : BaseRepositoryTests
{
    private sealed class FakeCurrentUserContext : ICurrentUserContext
    {
        public int EmployeeId { get; }

        public FakeCurrentUserContext(int employeeId)
        {
            EmployeeId = employeeId;
        }
    }

    private StockMovementService CreateService(int employeeId = 1)
    {
        return new StockMovementService(
            UnitOfWork,
            Log.Logger,
            new FakeCurrentUserContext(employeeId));
    }


    // =========================================================
    // Constructor
    // =========================================================

    [Test]
    public void Constructor_WhenUnitOfWorkIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var currentUserContext =
            new FakeCurrentUserContext(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new StockMovementService(
                null!,
                Log.Logger,
                currentUserContext));
    }

    [Test]
    public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var currentUserContext =
            new FakeCurrentUserContext(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new StockMovementService(
                UnitOfWork,
                null!,
                currentUserContext));
    }

    [Test]
    public void Constructor_WhenCurrentUserContextIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new StockMovementService(
                UnitOfWork,
                Log.Logger,
                null!));
    }


    // =========================================================
    // GetByProductId
    // =========================================================

    [Test]
    public void GetByProductId_WhenMovementsExist_ShouldReturnProductMovements()
    {
        // Arrange
        var service = CreateService(employeeId: 1);

        service.Refill(
            productId: 1,
            quantity: 5);

        // Act
        var result = service
            .GetByProductId(1)
            .ToList();

        // Assert
        Assert.That(result, Is.Not.Empty);

        Assert.That(
            result.All(x => x.ProductId == 1),
            Is.True);

        Assert.That(
            result.Any(x =>
                x.ProductId == 1 &&
                x.QuantityChange == 5 &&
                x.ChangedByEmployeeId == 1),
            Is.True);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetByProductId_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.GetByProductId(productId));
    }

    [Test]
    public void GetByProductId_WhenNoMovementsExist_ShouldReturnEmpty()
    {
        // Arrange
        var service = CreateService();

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "Product Without Stock".AddGuid(),
            Price = 100m
        };

        var productId =
            UnitOfWork.ProductRepository.Insert(product);

        // Act
        var result = service
            .GetByProductId(productId)
            .ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }


    // =========================================================
    // GetByDateRange
    // =========================================================

    [Test]
    public void GetByDateRange_WhenMovementsExist_ShouldReturnMovementsInsideRange()
    {
        // Arrange
        var service = CreateService(employeeId: 1);

        var from = DateTime.Now.AddMinutes(-1);

        service.Refill(
            productId: 1,
            quantity: 4);

        var to = DateTime.Now.AddMinutes(1);

        // Act
        var result = service
            .GetByDateRange(from, to)
            .ToList();

        // Assert
        Assert.That(result, Is.Not.Empty);

        Assert.That(
            result.Any(x =>
                x.ProductId == 1 &&
                x.QuantityChange == 4),
            Is.True);

        Assert.That(
            result.All(x =>
                x.CreatedDate >= from &&
                x.CreatedDate <= to),
            Is.True);
    }

    [Test]
    public void GetByDateRange_WhenFromIsGreaterThanTo_ShouldThrowArgumentException()
    {
        // Arrange
        var service = CreateService();

        var from = DateTime.Now;
        var to = from.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.GetByDateRange(from, to));
    }

    [Test]
    public void GetByDateRange_WhenNoMovementsExist_ShouldReturnEmpty()
    {
        // Arrange
        var service = CreateService();

        var from = DateTime.Now.AddYears(-10);
        var to = DateTime.Now.AddYears(-9);

        // Act
        var result = service
            .GetByDateRange(from, to)
            .ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }


    // =========================================================
    // GetByEmployeeId
    // =========================================================

    [Test]
    public void GetByEmployeeId_WhenMovementsExist_ShouldReturnEmployeeMovements()
    {
        // Arrange
        const int employeeId = 1;

        var service = CreateService(employeeId);

        service.Refill(
            productId: 1,
            quantity: 6);

        // Act
        var result = service
            .GetByEmployeeId(employeeId)
            .ToList();

        // Assert
        Assert.That(result, Is.Not.Empty);

        Assert.That(
            result.All(x =>
                x.ChangedByEmployeeId == employeeId),
            Is.True);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetByEmployeeId_WhenEmployeeIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int employeeId)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.GetByEmployeeId(employeeId));
    }


    // =========================================================
    // GetOutOfStockProducts
    // =========================================================

    [Test]
    public void GetOutOfStockProducts_WhenProductHasNoStock_ShouldReturnProduct()
    {
        // Arrange
        var service = CreateService();

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "Out Of Stock Product".AddGuid(),
            Price = 100m
        };

        var productId =
            UnitOfWork.ProductRepository.Insert(product);

        // Act
        var result = service
            .GetOutOfStockProducts()
            .ToList();

        // Assert
        Assert.That(
            result.Any(x =>
                x.ProductId == productId),
            Is.True);

        var stock = result.Single(x =>
            x.ProductId == productId);

        Assert.That(
            stock.CurrentQuantity,
            Is.LessThanOrEqualTo(0));
    }


    // =========================================================
    // Refill
    // =========================================================

    [Test]
    public void Refill_WithValidData_ShouldCreateRefillMovement()
    {
        // Arrange
        const int employeeId = 1;

        var service = CreateService(employeeId);

        var existingIds = service
            .GetByProductId(1)
            .Select(x => x.Id)
            .ToHashSet();

        var beforeMovements = service
            .GetByProductId(1)
            .OrderBy(x => x.CreatedDate)
            .ThenBy(x => x.Id)
            .ToList();

        var currentStock = beforeMovements.Count == 0
            ? 0
            : beforeMovements.Last().QuantityBefore +
              beforeMovements.Last().QuantityChange;

        // Act
        service.Refill(
            productId: 1,
            quantity: 10);

        // Assert
        var movement = service
            .GetByProductId(1)
            .Single(x => !existingIds.Contains(x.Id));

        Assert.Multiple(() =>
        {
            Assert.That(
                movement.ProductId,
                Is.EqualTo(1));

            Assert.That(
                movement.MovementType,
                Is.EqualTo(1));

            Assert.That(
                movement.QuantityBefore,
                Is.EqualTo(currentStock));

            Assert.That(
                movement.QuantityChange,
                Is.EqualTo(10));

            Assert.That(
                movement.ChangedByEmployeeId,
                Is.EqualTo(employeeId));
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Refill_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.Refill(
                productId,
                10));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Refill_WhenQuantityIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int quantity)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.Refill(
                1,
                quantity));
    }

    [Test]
    public void Refill_WhenProductDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.Refill(
                int.MaxValue,
                10));
    }

    [Test]
    public void Refill_WhenProductIsDeleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var service = CreateService();

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "Deleted Stock Product".AddGuid(),
            Price = 100m
        };

        var productId =
            UnitOfWork.ProductRepository.Insert(product);

        UnitOfWork.ProductRepository.Delete(productId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.Refill(
                productId,
                10));
    }


    // =========================================================
    // Adjust
    // =========================================================

    [Test]
    public void Adjust_WithPositiveDifference_ShouldIncreaseStock()
    {
        // Arrange
        const int employeeId = 1;

        var service = CreateService(employeeId);

        service.Refill(
            productId: 1,
            quantity: 10);

        var beforeMovement = service
            .GetByProductId(1)
            .OrderBy(x => x.CreatedDate)
            .ThenBy(x => x.Id)
            .Last();

        var stockBefore =
            beforeMovement.QuantityBefore +
            beforeMovement.QuantityChange;

        var existingIds = service
            .GetByProductId(1)
            .Select(x => x.Id)
            .ToHashSet();

        // Act
        service.Adjust(
            productId: 1,
            quantityDifference: 3,
            reason: "Positive adjustment");

        // Assert
        var movement = service
            .GetByProductId(1)
            .Single(x => !existingIds.Contains(x.Id));

        Assert.Multiple(() =>
        {
            Assert.That(
                movement.ProductId,
                Is.EqualTo(1));

            Assert.That(
                movement.MovementType,
                Is.EqualTo(2));

            Assert.That(
                movement.QuantityBefore,
                Is.EqualTo(stockBefore));

            Assert.That(
                movement.QuantityChange,
                Is.EqualTo(3));

            Assert.That(
                movement.ChangedByEmployeeId,
                Is.EqualTo(employeeId));

            Assert.That(
                movement.Reason,
                Is.EqualTo("Positive adjustment"));
        });
    }

    [Test]
    public void Adjust_WithNegativeDifference_ShouldDecreaseStock()
    {
        // Arrange
        const int employeeId = 1;

        var service = CreateService(employeeId);

        service.Refill(
            productId: 1,
            quantity: 10);

        var beforeMovement = service
            .GetByProductId(1)
            .OrderBy(x => x.CreatedDate)
            .ThenBy(x => x.Id)
            .Last();

        var stockBefore =
            beforeMovement.QuantityBefore +
            beforeMovement.QuantityChange;

        var existingIds = service
            .GetByProductId(1)
            .Select(x => x.Id)
            .ToHashSet();

        // Act
        service.Adjust(
            productId: 1,
            quantityDifference: -3,
            reason: "Damaged products");

        // Assert
        var movement = service
            .GetByProductId(1)
            .Single(x => !existingIds.Contains(x.Id));

        Assert.Multiple(() =>
        {
            Assert.That(
                movement.ProductId,
                Is.EqualTo(1));

            Assert.That(
                movement.MovementType,
                Is.EqualTo(2));

            Assert.That(
                movement.QuantityBefore,
                Is.EqualTo(stockBefore));

            Assert.That(
                movement.QuantityChange,
                Is.EqualTo(-3));

            Assert.That(
                movement.ChangedByEmployeeId,
                Is.EqualTo(employeeId));

            Assert.That(
                movement.QuantityBefore +
                movement.QuantityChange,
                Is.EqualTo(stockBefore - 3));

            Assert.That(
                movement.Reason,
                Is.EqualTo("Damaged products"));
        });
    }

    [Test]
    public void Adjust_WhenDifferenceIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.Adjust(
                1,
                0,
                "Adjustment"));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Adjust_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.Adjust(
                productId,
                1,
                "Adjustment"));
    }

    [Test]
    public void Adjust_WhenProductDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.Adjust(
                int.MaxValue,
                1,
                "Adjustment"));
    }

    [Test]
    public void Adjust_WhenStockWouldBecomeNegative_ShouldThrowSqlException()
    {
        // Arrange
        var service = CreateService(employeeId: 1);

        var product = new ProductDTO
        {
            CategoryId = 1,
            ProductName = "Zero Stock Product".AddGuid(),
            Price = 100m
        };

        var productId =
            UnitOfWork.ProductRepository.Insert(product);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.Adjust(
                productId,
                -1,
                "Negative adjustment"));
    }
}