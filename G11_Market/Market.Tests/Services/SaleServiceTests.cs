using Market.DTO;
using Market.DTO.Enums;
using Market.Services;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Services;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Market.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SaleServiceTests : BaseRepositoryTests
{
    private sealed class FakeCurrentUserContext : ICurrentUserContext
    {
        public int EmployeeId { get; }

        public FakeCurrentUserContext(int employeeId)
        {
            EmployeeId = employeeId;
        }
    }

    private SaleService CreateSaleService(int employeeId = 1)
    {
        return new SaleService(
            UnitOfWork,
            Log.Logger,
            new FakeCurrentUserContext(employeeId));
    }

    private StockMovementService CreateStockMovementService(
        int employeeId = 1)
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
        var currentUserContext =
            new FakeCurrentUserContext(1);

        Assert.Throws<ArgumentNullException>(() =>
            new SaleService(
                null!,
                Log.Logger,
                currentUserContext));
    }

    [Test]
    public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()
    {
        var currentUserContext =
            new FakeCurrentUserContext(1);

        Assert.Throws<ArgumentNullException>(() =>
            new SaleService(
                UnitOfWork,
                null!,
                currentUserContext));
    }

    [Test]
    public void Constructor_WhenCurrentUserContextIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SaleService(
                UnitOfWork,
                Log.Logger,
                null!));
    }


    // =========================================================
    // GetById
    // =========================================================

    [Test]
    public void GetById_WhenSaleExists_ShouldReturnSale()
    {
        // Arrange
        var service = CreateSaleService();

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = 1
            });

        // Act
        var result = service.GetById(saleId);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(
                result!.Id,
                Is.EqualTo(saleId));

            Assert.That(
                result.CreatedEmployeeId,
                Is.EqualTo(1));

            Assert.That(
                result.Status,
                Is.EqualTo(SaleStatus.Draft));
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetById_WhenSaleIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int saleId)
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.GetById(saleId));
    }

    [Test]
    public void GetById_WhenSaleDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var service = CreateSaleService();

        // Act
        var result = service.GetById(int.MaxValue);

        // Assert
        Assert.That(result, Is.Null);
    }


    // =========================================================
    // CompleteSale
    // =========================================================

    [Test]
    public void CompleteSale_WhenSaleIsValid_ShouldCompleteSale()
    {
        // Arrange
        const int employeeId = 1;

        var service =
            CreateSaleService(employeeId);

        var stockService =
            CreateStockMovementService(employeeId);

        stockService.Refill(
            productId: 1,
            quantity: 20);

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = employeeId
            });

        service.AddItem(
            saleId,
            productId: 1,
            quantity: 2);

        // Act
        service.CompleteSale(saleId);

        // Assert
        var result = service.GetById(saleId);

        Assert.That(result, Is.Not.Null);

        Assert.That(
            result!.Status,
            Is.EqualTo(SaleStatus.Completed));
    }

    [Test]
    public void CompleteSale_ShouldDecreaseStock()
    {
        // Arrange
        const int employeeId = 1;
        const int productId = 1;

        var service =
            CreateSaleService(employeeId);

        var stockService =
            CreateStockMovementService(employeeId);

        stockService.Refill(
            productId,
            20);

        var beforeMovement = stockService
            .GetByProductId(productId)
            .OrderBy(x => x.CreatedDate)
            .ThenBy(x => x.Id)
            .Last();

        var stockBefore =
            beforeMovement.QuantityBefore +
            beforeMovement.QuantityChange;

        var existingMovementIds = stockService
            .GetByProductId(productId)
            .Select(x => x.Id)
            .ToHashSet();

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = employeeId
            });

        service.AddItem(
            saleId,
            productId,
            quantity: 2);

        // Act
        service.CompleteSale(saleId);

        // Assert
        var movement = stockService
            .GetByProductId(productId)
            .Single(x => !existingMovementIds.Contains(x.Id));

        Assert.Multiple(() =>
        {
            Assert.That(
                movement.ProductId,
                Is.EqualTo(productId));

            Assert.That(
                movement.MovementType,
                Is.EqualTo(0));

            Assert.That(
                movement.QuantityBefore,
                Is.EqualTo(stockBefore));

            Assert.That(
                movement.QuantityChange,
                Is.EqualTo(-2));

            Assert.That(
                movement.ChangedByEmployeeId,
                Is.EqualTo(employeeId));

            Assert.That(
                movement.QuantityBefore +
                movement.QuantityChange,
                Is.EqualTo(stockBefore - 2));
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void CompleteSale_WhenSaleIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int saleId)
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.CompleteSale(saleId));
    }

    [Test]
    public void CompleteSale_WhenSaleDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.CompleteSale(int.MaxValue));
    }

    [Test]
    public void CompleteSale_WhenSaleIsAlreadyCompleted_ShouldThrowSqlException()
    {
        // Arrange
        const int employeeId = 1;

        var service =
            CreateSaleService(employeeId);

        var stockService =
            CreateStockMovementService(employeeId);

        stockService.Refill(
            productId: 1,
            quantity: 20);

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = employeeId
            });

        service.AddItem(
            saleId,
            productId: 1,
            quantity: 1);

        service.CompleteSale(saleId);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.CompleteSale(saleId));
    }


    // =========================================================
    // AddItem
    // =========================================================

    [Test]
    public void AddItem_WithValidData_ShouldAddItemToSale()
    {
        // Arrange
        var service = CreateSaleService();

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = 1
            });

        var product =
            UnitOfWork.ProductRepository.GetById(1)!;

        // Act
        service.AddItem(
            saleId,
            productId: 1,
            quantity: 2);

        // Assert
        var result = UnitOfWork.SaleItemRepository
            .GetBySaleId(saleId)
            .Single();

        Assert.Multiple(() =>
        {
            Assert.That(
                result.SaleId,
                Is.EqualTo(saleId));

            Assert.That(
                result.ProductId,
                Is.EqualTo(1));

            Assert.That(
                result.Quantity,
                Is.EqualTo(2));

            Assert.That(
                result.UnitPrice,
                Is.EqualTo(product.Price));
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void AddItem_WhenSaleIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int saleId)
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddItem(
                saleId,
                1,
                1));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void AddItem_WhenProductIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int productId)
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddItem(
                1,
                productId,
                1));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void AddItem_WhenQuantityIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int quantity)
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddItem(
                1,
                1,
                quantity));
    }

    [Test]
    public void AddItem_WhenSaleDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.AddItem(
                int.MaxValue,
                1,
                1));
    }

    [Test]
    public void AddItem_WhenProductDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var service = CreateSaleService();

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = 1
            });

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.AddItem(
                saleId,
                int.MaxValue,
                1));
    }

    [Test]
    public void AddItem_WhenProductAlreadyExistsInSale_ShouldThrowSqlException()
    {
        // Arrange
        var service = CreateSaleService();

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = 1
            });

        service.AddItem(
            saleId,
            productId: 1,
            quantity: 1);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.AddItem(
                saleId,
                productId: 1,
                quantity: 1));
    }


    // =========================================================
    // UpdateItemQuantity
    // =========================================================

    [Test]
    public void UpdateItemQuantity_WithValidData_ShouldUpdateQuantity()
    {
        // Arrange
        var service = CreateSaleService();

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = 1
            });

        service.AddItem(
            saleId,
            productId: 1,
            quantity: 1);

        var saleItem = UnitOfWork.SaleItemRepository
            .GetBySaleId(saleId)
            .Single();

        // Act
        service.UpdateItemQuantity(
            saleItem.Id,
            quantity: 5);

        // Assert
        var result = UnitOfWork.SaleItemRepository
            .GetById(saleItem.Id);

        Assert.That(result, Is.Not.Null);

        Assert.That(
            result!.Quantity,
            Is.EqualTo(5));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void UpdateItemQuantity_WhenSaleItemIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int saleItemId)
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.UpdateItemQuantity(
                saleItemId,
                1));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void UpdateItemQuantity_WhenQuantityIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int quantity)
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.UpdateItemQuantity(
                1,
                quantity));
    }

    [Test]
    public void UpdateItemQuantity_WhenSaleItemDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.UpdateItemQuantity(
                int.MaxValue,
                1));
    }

    [Test]
    public void UpdateItemQuantity_WhenSaleIsCompleted_ShouldThrowSqlException()
    {
        // Arrange
        const int employeeId = 1;

        var service =
            CreateSaleService(employeeId);

        var stockService =
            CreateStockMovementService(employeeId);

        stockService.Refill(
            1,
            20);

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = employeeId
            });

        service.AddItem(
            saleId,
            1,
            1);

        var saleItem = UnitOfWork.SaleItemRepository
            .GetBySaleId(saleId)
            .Single();

        service.CompleteSale(saleId);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            service.UpdateItemQuantity(
                saleItem.Id,
                2));
    }


    // =========================================================
    // RemoveItem
    // =========================================================

    [Test]
    public void RemoveItem_WhenSaleItemExists_ShouldRemoveItem()
    {
        // Arrange
        var service = CreateSaleService();

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = 1
            });

        service.AddItem(
            saleId,
            1,
            1);

        var saleItem = UnitOfWork.SaleItemRepository
            .GetBySaleId(saleId)
            .Single();

        // Act
        service.RemoveItem(saleItem.Id);

        // Assert
        var result = UnitOfWork.SaleItemRepository
            .GetBySaleId(saleId)
            .ToList();

        Assert.That(result, Is.Empty);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void RemoveItem_WhenSaleItemIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int saleItemId)
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.RemoveItem(saleItemId));
    }

    [Test]
    public void RemoveItem_WhenSaleItemDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.RemoveItem(int.MaxValue));
    }


    // =========================================================
    // GetByEmployee
    // =========================================================

    [Test]
    public void GetByEmployee_WhenSalesExist_ShouldReturnEmployeeSales()
    {
        // Arrange
        var service = CreateSaleService();

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = 1
            });

        // Act
        var result = service
            .GetByEmployee(1)
            .ToList();

        // Assert
        Assert.That(
            result.Any(x => x.Id == saleId),
            Is.True);

        Assert.That(
            result.All(x =>
                x.CreatedEmployeeId == 1),
            Is.True);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetByEmployee_WhenEmployeeIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int employeeId)
    {
        // Arrange
        var service = CreateSaleService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.GetByEmployee(employeeId));
    }


    // =========================================================
    // GetByStatus
    // =========================================================

    [Test]
    public void GetByStatus_WhenDraftSalesExist_ShouldReturnDraftSales()
    {
        // Arrange
        var service = CreateSaleService();

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = 1
            });

        // Act
        var result = service
            .GetByStatus(SaleStatus.Draft)
            .ToList();

        // Assert
        Assert.That(
            result.Any(x => x.Id == saleId),
            Is.True);

        Assert.That(
            result.All(x =>
                x.Status == SaleStatus.Draft),
            Is.True);
    }

    [Test]
    public void GetByStatus_WhenStatusIsInvalid_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var service = CreateSaleService();

        var invalidStatus = (SaleStatus)99;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.GetByStatus(invalidStatus));
    }


    // =========================================================
    // GetByDateRange
    // =========================================================

    [Test]
    public void GetByDateRange_WhenSaleExistsInsideRange_ShouldReturnSale()
    {
        // Arrange
        var service = CreateSaleService();

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = 1
            });

        var insertedSale =
            UnitOfWork.SaleRepository.GetById(saleId);

        Assert.That(insertedSale, Is.Not.Null);

        var from =
            insertedSale!.CreatedDate.AddMinutes(-1);

        var to =
            insertedSale.CreatedDate.AddMinutes(1);

        // Act
        var result = service
            .GetByDateRange(from, to)
            .ToList();

        // Assert
        Assert.That(
            result.Any(x => x.Id == saleId),
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
        var service = CreateSaleService();

        var from = DateTime.Now;
        var to = from.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.GetByDateRange(from, to));
    }


    // =========================================================
    // GetDailyIncome
    // =========================================================

    [Test]
    public void GetDailyIncome_WhenSaleIsCompleted_ShouldIncludeSaleTotal()
    {
        // Arrange
        const int employeeId = 1;

        var service =
            CreateSaleService(employeeId);

        var stockService =
            CreateStockMovementService(employeeId);

        stockService.Refill(
            productId: 1,
            quantity: 20);

        var product =
            UnitOfWork.ProductRepository.GetById(1)!;

        var date = DateTime.Today;

        var incomeBefore =
            service.GetDailyIncome(date);

        var saleId = UnitOfWork.SaleRepository.Insert(
            new SaleDTO
            {
                CreatedEmployeeId = employeeId
            });

        service.AddItem(
            saleId,
            productId: 1,
            quantity: 2);

        // Act
        service.CompleteSale(saleId);

        var incomeAfter =
            service.GetDailyIncome(date);

        // Assert
        Assert.That(
            incomeAfter - incomeBefore,
            Is.EqualTo(product.Price * 2));
    }
}