using Market.DTO;
using Market.DTO.Enums;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class SaleRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void GetSalesByEmployee_ShouldReturnOnlyEmployeeSales()
    {
        // Arrange
        const int employeeId = 2;

        // Act
        var sales = UnitOfWork.SaleRepository
            .GetSalesByEmployee(employeeId)
            .ToList();

        // Assert
        Assert.That(sales, Is.Not.Empty);

        Assert.That(
            sales.All(s => s.CreatedEmployeeId == employeeId),
            Is.True);
    }

    [Test]
    public void GetSalesByEmployee_WhenEmployeeIdIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.SaleRepository
                .GetSalesByEmployee(0)
                .ToList());
    }

    [Test]
    public void GetSalesByStatus_ShouldReturnOnlyRequestedStatus()
    {
        // Arrange
        var status = SaleStatus.Draft;

        // Act
        var sales = UnitOfWork.SaleRepository
            .GetSalesByStatus(status)
            .ToList();

        // Assert
        Assert.That(sales, Is.Not.Empty);

        Assert.That(
            sales.All(s => s.Status == status),
            Is.True);
    }

    [Test]
    public void GetSalesByStatus_WhenStatusIsInvalid_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var invalidStatus = (SaleStatus)99;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.SaleRepository
                .GetSalesByStatus(invalidStatus)
                .ToList());
    }

    [Test]
    public void GetSalesByDateRange_ShouldReturnSalesInsideRange()
    {
        // Arrange
        var dateFrom = DateTime.UtcNow.AddMinutes(-5);
        var dateTo = DateTime.UtcNow.AddMinutes(5);

        // Act
        var sales = UnitOfWork.SaleRepository
            .GetSalesByDateRange(dateFrom, dateTo)
            .ToList();

        // Assert
        Assert.That(sales, Is.Not.Empty);

        Assert.That(
            sales.All(s =>
                s.CreatedDate >= dateFrom &&
                s.CreatedDate <= dateTo),
            Is.True);
    }

    [Test]
    public void GetSalesByDateRange_WhenRangeInvalid_ShouldThrowArgumentException()
    {
        // Arrange
        var dateFrom = DateTime.UtcNow;
        var dateTo = dateFrom.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            UnitOfWork.SaleRepository
                .GetSalesByDateRange(dateFrom, dateTo)
                .ToList());
    }

    [Test]
    public void GetSalesByDateRange_WhenNoSalesExist_ShouldReturnEmpty()
    {
        // Arrange
        var dateFrom = DateTime.UtcNow.AddYears(-10);
        var dateTo = DateTime.UtcNow.AddYears(-9);

        // Act
        var sales = UnitOfWork.SaleRepository
            .GetSalesByDateRange(dateFrom, dateTo)
            .ToList();

        // Assert
        Assert.That(sales, Is.Empty);
    }

    [Test]
    public void CancelSale_ShouldCancelDraftSale()
    {
        // Arrange
        var sale = SalesTestDataFactory.CreateSale(
            employeeId: 2,
            status: SaleStatus.Draft);

        var saleId =
            UnitOfWork.SaleRepository.Insert(sale);

        const int employeeId = 1;
        const string cancelReason = "Customer cancelled sale";

        // Act
        UnitOfWork.SaleRepository.CancelSale(
            saleId,
            employeeId,
            cancelReason);

        // Assert
        var cancelledSale =
            UnitOfWork.SaleRepository.GetById(saleId);

        Assert.That(cancelledSale, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(
                cancelledSale!.Status,
                Is.EqualTo(SaleStatus.Cancelled));

            Assert.That(
                cancelledSale.CancelledByEmployeeId,
                Is.EqualTo(employeeId));

            Assert.That(
                cancelledSale.CancelReason,
                Is.EqualTo(cancelReason));

            Assert.That(
                cancelledSale.CancelledDate,
                Is.Not.Null);
        });
    }

    [Test]
    public void CancelSale_WhenCompleted_ShouldThrowSqlException()
    {
        // Arrange
        const int completedSaleId = 1;
        const int employeeId = 1;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleRepository.CancelSale(
                completedSaleId,
                employeeId,
                "Trying to cancel completed sale"));
    }

    [Test]
    public void CancelSale_WhenAlreadyCancelled_ShouldThrowSqlException()
    {
        // Arrange
        const int cancelledSaleId = 3;
        const int employeeId = 1;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleRepository.CancelSale(
                cancelledSaleId,
                employeeId,
                "Trying to cancel again"));
    }

    [Test]
    public void CancelSale_WhenSaleIdIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.SaleRepository.CancelSale(
                0,
                1,
                "Test reason"));
    }

    [Test]
    public void CancelSale_WhenEmployeeIdIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.SaleRepository.CancelSale(
                2,
                0,
                "Test reason"));
    }

    [Test]
    public void CancelSale_WhenReasonIsWhitespace_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            UnitOfWork.SaleRepository.CancelSale(
                2,
                1,
                "   "));
    }

    [Test]
    public void CompleteSale_ShouldCompleteDraftSale()
    {
        // Arrange
        const int employeeId = 3;

        var sale = SalesTestDataFactory.CreateSale(
            employeeId: employeeId,
            status: SaleStatus.Draft);

        var saleId =
            UnitOfWork.SaleRepository.Insert(sale);

        var saleItem = new SaleItemDTO
        {
            SaleId = saleId,
            ProductId = 1,
            Quantity = 1,
            UnitPrice = 1200,
            DiscountAmount = 0
        };

        UnitOfWork.SaleItemRepository.Insert(saleItem);

        // Act
        UnitOfWork.SaleRepository.CompleteSale(
            saleId,
            employeeId);

        // Assert
        var completedSale =
            UnitOfWork.SaleRepository.GetById(saleId);

        Assert.That(completedSale, Is.Not.Null);

        Assert.That(
            completedSale!.Status,
            Is.EqualTo(SaleStatus.Completed));
    }

    [Test]
    public void CompleteSale_ShouldCreateStockMovement()
    {
        // Arrange
        const int employeeId = 3;
        const int productId = 1;

        var sale = SalesTestDataFactory.CreateSale(
            employeeId: employeeId,
            status: SaleStatus.Draft);

        var saleId =
            UnitOfWork.SaleRepository.Insert(sale);

        var saleItem = new SaleItemDTO
        {
            SaleId = saleId,
            ProductId = productId,
            Quantity = 1,
            UnitPrice = 1200,
            DiscountAmount = 0
        };

        var saleItemId =
            UnitOfWork.SaleItemRepository.Insert(saleItem);

        var movementIdsBefore =
            UnitOfWork.StockMovementRepository
                .GetByProductId(productId)
                .Select(x => x.Id)
                .ToHashSet();

        // Act
        UnitOfWork.SaleRepository.CompleteSale(
            saleId,
            employeeId);

        // Assert
        var movement =
            UnitOfWork.StockMovementRepository
                .GetByProductId(productId)
                .Single(x => !movementIdsBefore.Contains(x.Id));

        Assert.Multiple(() =>
        {
            Assert.That(
                movement.ProductId,
                Is.EqualTo(productId));

            Assert.That(
                movement.MovementType,
                Is.EqualTo(0));

            Assert.That(
                movement.QuantityChange,
                Is.EqualTo(-1));

            Assert.That(
                movement.SaleItemId,
                Is.EqualTo(saleItemId));

            Assert.That(
                movement.ChangedByEmployeeId,
                Is.EqualTo(employeeId));
        });
    }

    [Test]
    public void CompleteSale_WhenSaleHasNoItems_ShouldThrowSqlException()
    {
        // Arrange
        const int employeeId = 3;

        var sale = SalesTestDataFactory.CreateSale(
            employeeId: employeeId,
            status: SaleStatus.Draft);

        var saleId =
            UnitOfWork.SaleRepository.Insert(sale);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleRepository.CompleteSale(
                saleId,
                employeeId));

        var unchangedSale =
            UnitOfWork.SaleRepository.GetById(saleId);

        Assert.That(
            unchangedSale!.Status,
            Is.EqualTo(SaleStatus.Draft));
    }

    [Test]
    public void CompleteSale_WhenSaleIsAlreadyCompleted_ShouldThrowSqlException()
    {
        // Arrange
        const int completedSaleId = 1;
        const int employeeId = 1;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleRepository.CompleteSale(
                completedSaleId,
                employeeId));
    }

    [Test]
    public void CompleteSale_WhenSaleIsCancelled_ShouldThrowSqlException()
    {
        // Arrange
        const int cancelledSaleId = 3;
        const int employeeId = 1;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleRepository.CompleteSale(
                cancelledSaleId,
                employeeId));
    }

    [Test]
    public void CompleteSale_WhenStockIsInsufficient_ShouldThrowSqlException()
    {
        // Arrange
        const int employeeId = 3;
        const int productId = 1;

        var sale = SalesTestDataFactory.CreateSale(
            employeeId: employeeId,
            status: SaleStatus.Draft);

        var saleId =
            UnitOfWork.SaleRepository.Insert(sale);

        var saleItem = new SaleItemDTO
        {
            SaleId = saleId,
            ProductId = productId,
            Quantity = 100000,
            UnitPrice = 1200,
            DiscountAmount = 0
        };

        UnitOfWork.SaleItemRepository.Insert(saleItem);

        var movementCountBefore =
            UnitOfWork.StockMovementRepository
                .GetByProductId(productId)
                .Count();

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleRepository.CompleteSale(
                saleId,
                employeeId));

        var saleAfter =
            UnitOfWork.SaleRepository.GetById(saleId);

        var movementCountAfter =
            UnitOfWork.StockMovementRepository
                .GetByProductId(productId)
                .Count();

        Assert.Multiple(() =>
        {
            Assert.That(
                saleAfter!.Status,
                Is.EqualTo(SaleStatus.Draft));

            Assert.That(
                movementCountAfter,
                Is.EqualTo(movementCountBefore));
        });
    }

    [Test]
    public void CompleteSale_WhenSaleIdIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.SaleRepository.CompleteSale(
                0,
                1));
    }

    [Test]
    public void CompleteSale_WhenEmployeeIdIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.SaleRepository.CompleteSale(
                2,
                0));
    }

    [Test]
    public void GetIncomeByDateRange_ShouldReturnIncome()
    {
        // Arrange
        var dateFrom = DateTime.UtcNow.AddDays(-1);
        var dateTo = DateTime.UtcNow.AddDays(1);

        // Act
        var income =
            UnitOfWork.SaleRepository.GetIncomeByDateRange(
                dateFrom,
                dateTo);

        // Assert
        Assert.That(income, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public void GetIncomeByDateRange_WhenRangeInvalid_ShouldThrowArgumentException()
    {
        // Arrange
        var dateFrom = DateTime.UtcNow;
        var dateTo = dateFrom.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            UnitOfWork.SaleRepository.GetIncomeByDateRange(
                dateFrom,
                dateTo));
    }
}