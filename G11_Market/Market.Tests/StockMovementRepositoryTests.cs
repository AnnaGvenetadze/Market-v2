//using Microsoft.Data.SqlClient;

//namespace Market.Tests;

//public sealed class StockMovementRepositoryTests : BaseRepositoryTests
//{
//    [Test]
//    public void GetByProductId_ShouldReturnOnlyProductMovements()
//    {
//        // Arrange
//        const int productId = 1;

//        // Act
//        var movements = UnitOfWork.StockMovementRepository
//            .GetByProductId(productId)
//            .ToList();

//        // Assert
//        Assert.That(movements, Is.Not.Empty);

//        Assert.That(
//            movements.All(movement => movement.ProductId == productId),
//            Is.True);
//    }

//    [Test]
//    public void GetByProductId_WhenProductIdIsZero_ShouldThrowArgumentOutOfRangeException()
//    {
//        // Arrange
//        const int productId = 0;

//        // Act & Assert
//        Assert.Throws<ArgumentOutOfRangeException>(() =>
//            UnitOfWork.StockMovementRepository
//                .GetByProductId(productId)
//                .ToList());
//    }

//    [Test]
//    public void GetByEmployeeId_ShouldReturnOnlyEmployeeMovements()
//    {
//        // Arrange
//        const int employeeId = 2;

//        // Act
//        var movements = UnitOfWork.StockMovementRepository
//            .GetByEmployeeId(employeeId)
//            .ToList();

//        // Assert
//        Assert.That(movements, Is.Not.Empty);

//        Assert.That(
//            movements.All(movement =>
//                movement.ChangedByEmployeeId == employeeId),
//            Is.True);
//    }

//    [Test]
//    public void GetByEmployeeId_WhenEmployeeIdIsZero_ShouldThrowArgumentOutOfRangeException()
//    {
//        // Arrange
//        const int employeeId = 0;

//        // Act & Assert
//        Assert.Throws<ArgumentOutOfRangeException>(() =>
//            UnitOfWork.StockMovementRepository
//                .GetByEmployeeId(employeeId)
//                .ToList());
//    }

//    [Test]
//    public void GetByDateRange_ShouldReturnMovementsInsideRange()
//    {
//        // Arrange
//        var from = DateTime.UtcNow.AddDays(-1);
//        var to = DateTime.UtcNow.AddDays(1);

//        // Act
//        var movements = UnitOfWork.StockMovementRepository
//            .GetByDateRange(from, to)
//            .ToList();

//        // Assert
//        Assert.That(movements, Is.Not.Empty);

//        Assert.That(
//            movements.All(movement =>
//                movement.CreatedDate >= from &&
//                movement.CreatedDate <= to),
//            Is.True);
//    }

//    [Test]
//    public void GetByDateRange_WhenRangeInvalid_ShouldThrowArgumentException()
//    {
//        // Arrange
//        var from = DateTime.UtcNow;
//        var to = from.AddDays(-1);

//        // Act & Assert
//        Assert.Throws<ArgumentException>(() =>
//            UnitOfWork.StockMovementRepository
//                .GetByDateRange(from, to)
//                .ToList());
//    }

//    [Test]
//    public void GetByDateRange_WhenNoMovementsExist_ShouldReturnEmpty()
//    {
//        // Arrange
//        var from = DateTime.UtcNow.AddYears(-10);
//        var to = DateTime.UtcNow.AddYears(-9);

//        // Act
//        var movements = UnitOfWork.StockMovementRepository
//            .GetByDateRange(from, to)
//            .ToList();

//        // Assert
//        Assert.That(movements, Is.Empty);
//    }

//    [Test]
//    public void Refill_ShouldCreateRefillMovement()
//    {
//        // Arrange
//        const int productId = 1;
//        const int quantity = 5;
//        const int employeeId = 2;

//        var before = UnitOfWork.StockMovementRepository
//            .GetByProductId(productId)
//            .OrderBy(movement => movement.Id)
//            .Last();

//        var expectedQuantityBefore =
//            before.QuantityBefore + before.QuantityChange;

//        // Act
//        UnitOfWork.StockMovementRepository.Refill(
//            productId,
//            quantity,
//            employeeId);

//        // Assert
//        var movement = UnitOfWork.StockMovementRepository
//            .GetByProductId(productId)
//            .OrderBy(movement => movement.Id)
//            .Last();

//        Assert.Multiple(() =>
//        {
//            Assert.That(movement.ProductId, Is.EqualTo(productId));
//            Assert.That(movement.MovementType, Is.EqualTo(1));
//            Assert.That(movement.QuantityChange, Is.EqualTo(quantity));
//            Assert.That(
//                movement.QuantityBefore,
//                Is.EqualTo(expectedQuantityBefore));
//            Assert.That(
//                movement.ChangedByEmployeeId,
//                Is.EqualTo(employeeId));
//            Assert.That(movement.SaleItemId, Is.Null);
//        });
//    }

//    [Test]
//    public void Refill_WhenQuantityIsZero_ShouldThrowArgumentOutOfRangeException()
//    {
//        Assert.Throws<ArgumentOutOfRangeException>(() =>
//            UnitOfWork.StockMovementRepository.Refill(
//                1,
//                0,
//                2));
//    }

//    [Test]
//    public void Refill_WhenProductIdIsZero_ShouldThrowArgumentOutOfRangeException()
//    {
//        Assert.Throws<ArgumentOutOfRangeException>(() =>
//            UnitOfWork.StockMovementRepository.Refill(
//                0,
//                5,
//                2));
//    }

//    [Test]
//    public void Refill_WhenEmployeeIdIsZero_ShouldThrowArgumentOutOfRangeException()
//    {
//        Assert.Throws<ArgumentOutOfRangeException>(() =>
//            UnitOfWork.StockMovementRepository.Refill(
//                1,
//                5,
//                0));
//    }

//    [Test]
//    public void Refill_WhenProductDoesNotExist_ShouldThrowSqlException()
//    {
//        Assert.Throws<SqlException>(() =>
//            UnitOfWork.StockMovementRepository.Refill(
//                int.MaxValue,
//                5,
//                2));
//    }

//    [Test]
//    public void Adjust_WithNegativeDifference_ShouldDecreaseStock()
//    {
//        // Arrange
//        const int productId = 1;
//        const int quantityDifference = -3;
//        const int employeeId = 2;
//        const string reason = "Damaged items";

//        var before = UnitOfWork.StockMovementRepository
//            .GetByProductId(productId)
//            .OrderBy(movement => movement.Id)
//            .Last();

//        var expectedQuantityBefore =
//            before.QuantityBefore + before.QuantityChange;

//        // Act
//        UnitOfWork.StockMovementRepository.Adjust(
//            productId,
//            quantityDifference,
//            employeeId,
//            reason);

//        // Assert
//        var movement = UnitOfWork.StockMovementRepository
//            .GetByProductId(productId)
//            .OrderBy(movement => movement.Id)
//            .Last();

//        Assert.Multiple(() =>
//        {
//            Assert.That(movement.ProductId, Is.EqualTo(productId));
//            Assert.That(movement.MovementType, Is.EqualTo(2));
//            Assert.That(
//                movement.QuantityBefore,
//                Is.EqualTo(expectedQuantityBefore));
//            Assert.That(
//                movement.QuantityChange,
//                Is.EqualTo(quantityDifference));
//            Assert.That(
//                movement.ChangedByEmployeeId,
//                Is.EqualTo(employeeId));
//            Assert.That(movement.Reason, Is.EqualTo(reason));
//            Assert.That(movement.SaleItemId, Is.Null);
//        });
//    }

//    [Test]
//    public void Adjust_WithPositiveDifference_ShouldIncreaseStock()
//    {
//        // Arrange
//        const int productId = 1;
//        const int quantityDifference = 4;
//        const int employeeId = 2;
//        const string reason = "Inventory correction";

//        // Act
//        UnitOfWork.StockMovementRepository.Adjust(
//            productId,
//            quantityDifference,
//            employeeId,
//            reason);

//        // Assert
//        var movement = UnitOfWork.StockMovementRepository
//            .GetByProductId(productId)
//            .OrderBy(movement => movement.Id)
//            .Last();

//        Assert.Multiple(() =>
//        {
//            Assert.That(movement.MovementType, Is.EqualTo(2));
//            Assert.That(
//                movement.QuantityChange,
//                Is.EqualTo(quantityDifference));
//            Assert.That(movement.Reason, Is.EqualTo(reason));
//        });
//    }

//    [Test]
//    public void Adjust_WhenDifferenceIsZero_ShouldThrowArgumentException()
//    {
//        Assert.Throws<ArgumentException>(() =>
//            UnitOfWork.StockMovementRepository.Adjust(
//                1,
//                0,
//                2,
//                "Adjustment"));
//    }

//    [Test]
//    public void Adjust_WhenReasonIsWhitespace_ShouldThrowArgumentException()
//    {
//        Assert.Throws<ArgumentException>(() =>
//            UnitOfWork.StockMovementRepository.Adjust(
//                1,
//                -1,
//                2,
//                "   "));
//    }

//    [Test]
//    public void Adjust_WhenProductDoesNotExist_ShouldThrowSqlException()
//    {
//        Assert.Throws<SqlException>(() =>
//            UnitOfWork.StockMovementRepository.Adjust(
//                int.MaxValue,
//                1,
//                2,
//                "Adjustment"));
//    }

//    [Test]
//    public void GetOutOfStockProducts_ShouldNotReturnProductsWithPositiveStock()
//    {
//        // Act
//        var products = UnitOfWork.StockMovementRepository
//            .GetOutOfStockProducts()
//            .ToList();

//        // Assert
//        Assert.That(
//            products.Any(product => product.ProductId == 1),
//            Is.False);
//    }
//}