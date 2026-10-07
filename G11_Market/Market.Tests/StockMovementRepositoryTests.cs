using Microsoft.Data.SqlClient;

namespace Market.Tests;

public sealed class StockMovementRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void GetByProductId_ShouldReturnOnlyProductMovements()
    {
        // Arrange
        const int productId = 1;

        // Act
        var movements = UnitOfWork.StockMovementRepository
            .GetByProductId(productId)
            .ToList();

        // Assert
        Assert.That(movements, Is.Not.Empty);

        Assert.That(
            movements.All(movement => movement.ProductId == productId),
            Is.True);
    }

    [Test]
    public void GetByProductId_WhenProductIdIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int productId = 0;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.StockMovementRepository
                .GetByProductId(productId)
                .ToList());
    }

    [Test]
    public void GetByEmployeeId_ShouldReturnOnlyEmployeeMovements()
    {
        // Arrange
        const int employeeId = 2;

        // Act
        var movements = UnitOfWork.StockMovementRepository
            .GetByEmployeeId(employeeId)
            .ToList();

        // Assert
        Assert.That(movements, Is.Not.Empty);

        Assert.That(
            movements.All(movement =>
                movement.ChangedByEmployeeId == employeeId),
            Is.True);
    }

    [Test]
    public void GetByEmployeeId_WhenEmployeeIdIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int employeeId = 0;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.StockMovementRepository
                .GetByEmployeeId(employeeId)
                .ToList());
    }

    [Test]
    public void GetByDateRange_ShouldReturnMovementsInsideRange()
    {
        // Arrange
        var from = DateTime.UtcNow.AddDays(-1);
        var to = DateTime.UtcNow.AddDays(1);

        // Act
        var movements = UnitOfWork.StockMovementRepository
            .GetByDateRange(from, to)
            .ToList();

        // Assert
        Assert.That(movements, Is.Not.Empty);

        Assert.That(
            movements.All(movement =>
                movement.CreatedDate >= from &&
                movement.CreatedDate <= to),
            Is.True);
    }

    [Test]
    public void GetByDateRange_WhenRangeInvalid_ShouldThrowArgumentException()
    {
        // Arrange
        var from = DateTime.UtcNow;
        var to = from.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            UnitOfWork.StockMovementRepository
                .GetByDateRange(from, to)
                .ToList());
    }

    [Test]
    public void GetByDateRange_WhenNoMovementsExist_ShouldReturnEmpty()
    {
        // Arrange
        var from = DateTime.UtcNow.AddYears(-10);
        var to = DateTime.UtcNow.AddYears(-9);

        // Act
        var movements = UnitOfWork.StockMovementRepository
            .GetByDateRange(from, to)
            .ToList();

        // Assert
        Assert.That(movements, Is.Empty);
    }

    [Test]
    public void Refill_WhenQuantityIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.StockMovementRepository.Refill(
                1,
                0,
                2));
    }

    [Test]
    public void Refill_WhenProductIdIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.StockMovementRepository.Refill(
                0,
                5,
                2));
    }

    [Test]
    public void Refill_WhenEmployeeIdIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.StockMovementRepository.Refill(
                1,
                5,
                0));
    }

    [Test]
    public void Refill_WhenProductDoesNotExist_ShouldThrowSqlException()
    {
        Assert.Throws<SqlException>(() =>
            UnitOfWork.StockMovementRepository.Refill(
                int.MaxValue,
                5,
                2));
    }




    [Test]
    public void Adjust_WhenReasonIsWhitespace_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            UnitOfWork.StockMovementRepository.Adjust(
                1,
                -1,
                2,
                "   "));
    }

    [Test]
    public void Adjust_WhenProductDoesNotExist_ShouldThrowSqlException()
    {
        Assert.Throws<SqlException>(() =>
            UnitOfWork.StockMovementRepository.Adjust(
                int.MaxValue,
                1,
                2,
                "Adjustment"));
    }

    [Test]
    public void GetOutOfStockProducts_ShouldNotReturnProductsWithPositiveStock()
    {
        // Act
        var products = UnitOfWork.StockMovementRepository
            .GetOutOfStockProducts()
            .ToList();

        // Assert
        Assert.That(
            products.Any(product => product.ProductId == 1),
            Is.False);
    }

    [Test]
    public void Adjust_WithNegativeDifference_ShouldDecreaseStock()
    {
        // Arrange
        const int productId = 1;
        const int employeeId = 2;
        const int quantityDifference = -3;

        var quantityBefore = UnitOfWork.StockMovementRepository
            .GetByProductId(productId)
            .Sum(x => x.QuantityChange);

        // Act
        UnitOfWork.StockMovementRepository.Adjust(
            productId,
            quantityDifference,
            employeeId,
            "Damaged products");

        // Assert
        var quantityAfter = UnitOfWork.StockMovementRepository
            .GetByProductId(productId)
            .Sum(x => x.QuantityChange);

        Assert.That(
            quantityAfter,
            Is.EqualTo(quantityBefore + quantityDifference));
    }

    [Test]
    public void Adjust_WithPositiveDifference_ShouldIncreaseStock()
    {
        // Arrange
        const int productId = 1;
        const int employeeId = 2;
        const int quantityDifference = 5;

        var quantityBefore = UnitOfWork.StockMovementRepository
            .GetByProductId(productId)
            .Sum(x => x.QuantityChange);

        // Act
        UnitOfWork.StockMovementRepository.Adjust(
            productId,
            quantityDifference,
            employeeId,
            "Stock correction");

        // Assert
        var quantityAfter = UnitOfWork.StockMovementRepository
            .GetByProductId(productId)
            .Sum(x => x.QuantityChange);

        Assert.That(
            quantityAfter,
            Is.EqualTo(quantityBefore + quantityDifference));
    }

    [Test]
    public void Refill_ShouldCreateRefillMovement()
    {
        // Arrange
        const int productId = 1;
        const int employeeId = 2;
        const int quantity = 10;

        var existingMovementIds = UnitOfWork.StockMovementRepository
            .GetByProductId(productId)
            .Select(x => x.Id)
            .ToHashSet();

        // Act
        UnitOfWork.StockMovementRepository.Refill(
            productId,
            quantity,
            employeeId);

        // Assert
        var newMovement = UnitOfWork.StockMovementRepository
            .GetByProductId(productId)
            .Single(x => !existingMovementIds.Contains(x.Id));

        Assert.Multiple(() =>
        {
            Assert.That(newMovement.ProductId, Is.EqualTo(productId));
            Assert.That(newMovement.QuantityChange, Is.EqualTo(quantity));
            Assert.That(newMovement.ChangedByEmployeeId, Is.EqualTo(employeeId));
            Assert.That(newMovement.MovementType, Is.EqualTo(1));
        });
    }

    [Test]
    public void Adjust_WhenDifferenceIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int productId = 1;
        const int employeeId = 1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.StockMovementRepository.Adjust(
                productId,
                0,
                employeeId,
                "Test adjustment"));

        Assert.That(
            exception.ParamName,
            Is.EqualTo("quantityDifference"));
    }
}