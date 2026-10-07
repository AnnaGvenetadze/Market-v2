//using Market.DTO;
//using Market.DTO.Enums;
//using Market.Tests.Helpers;
//using Microsoft.Data.SqlClient;

//namespace Market.Tests;

//public class SaleRepositoryTests : BaseRepositoryTests
//{
//    [Test]
//    public void GetSalesByEmployee_ShouldReturnOnlyEmployeeSales()
//    {
//        // Arrange
//        const int employeeId = 2;

//        // Act
//        var sales = UnitOfWork.SaleRepository
//            .GetSalesByEmployee(employeeId)
//            .ToList();

//        // Assert
//        Assert.That(sales, Is.Not.Empty);

//        Assert.That(
//            sales.All(s => s.CreatedEmployeeId == employeeId),
//            Is.True);
//    }

//    [Test]
//    public void GetSalesByStatus_ShouldReturnOnlyRequestedStatus()
//    {
//        // Arrange
//        var status = SaleStatus.Draft;

//        // Act
//        var sales = UnitOfWork.SaleRepository
//            .GetSalesByStatus(status)
//            .ToList();

//        // Assert
//        Assert.That(sales, Is.Not.Empty);

//        Assert.That(
//            sales.All(s => s.Status == status),
//            Is.True);
//    }

//    [Test]
//    public void GetSalesByStatus_WhenStatusIsInvalid_ShouldThrow()
//    {
//        // Arrange
//        var invalidStatus = (SaleStatus)99;

//        // Act & Assert
//        Assert.Throws<ArgumentOutOfRangeException>(() =>
//            UnitOfWork.SaleRepository
//                .GetSalesByStatus(invalidStatus)
//                .ToList());
//    }

//    [Test]
//    public void GetSalesByDateRange_ShouldReturnSalesInsideRange()
//    {
//        // Arrange
//        var dateFrom = DateTime.UtcNow.AddMinutes(-5);
//        var dateTo = DateTime.UtcNow.AddMinutes(5);

//        // Act
//        var sales = UnitOfWork.SaleRepository
//            .GetSalesByDateRange(dateFrom, dateTo)
//            .ToList();

//        // Assert
//        Assert.That(sales, Is.Not.Empty);

//        Assert.That(
//            sales.All(s =>
//                s.CreatedDate >= dateFrom &&
//                s.CreatedDate <= dateTo),
//            Is.True);
//    }

//    [Test]
//    public void GetSalesByDateRange_WhenRangeInvalid_ShouldThrow()
//    {
//        // Arrange
//        var dateFrom = DateTime.Now;
//        var dateTo = dateFrom.AddDays(-1);

//        // Act & Assert
//        Assert.Throws<ArgumentException>(() =>
//            UnitOfWork.SaleRepository
//                .GetSalesByDateRange(dateFrom, dateTo)
//                .ToList());
//    }

//    [Test]
//    public void GetSalesByDateRange_WhenNoSalesExist_ShouldReturnEmpty()
//    {
//        // Arrange
//        var dateFrom = DateTime.Now.AddYears(-10);
//        var dateTo = DateTime.Now.AddYears(-9);

//        // Act
//        var sales = UnitOfWork.SaleRepository
//            .GetSalesByDateRange(dateFrom, dateTo)
//            .ToList();

//        // Assert
//        Assert.That(sales, Is.Empty);
//    }

//    [Test]
//    public void CancelSale_ShouldCancelDraftSale()
//    {
//        // Arrange
//        var sale = SalesTestDataFactory.CreateSale(
//            employeeId: 2,
//            status: SaleStatus.Draft);

//        var saleId =
//            UnitOfWork.SaleRepository.Insert(sale);

//        const int employeeId = 1;
//        const string cancelReason = "Customer cancelled sale";

//        // Act
//        UnitOfWork.SaleRepository.CancelSale(
//            saleId,
//            employeeId,
//            cancelReason);

//        // Assert
//        var cancelledSale =
//            UnitOfWork.SaleRepository.GetById(saleId);

//        Assert.That(cancelledSale, Is.Not.Null);

//        Assert.Multiple(() =>
//        {
//            Assert.That(
//                cancelledSale!.Status,
//                Is.EqualTo(SaleStatus.Cancelled));

//            Assert.That(
//                cancelledSale.CancelledByEmployeeId,
//                Is.EqualTo(employeeId));

//            Assert.That(
//                cancelledSale.CancelReason,
//                Is.EqualTo(cancelReason));

//            Assert.That(
//                cancelledSale.CancelledDate,
//                Is.Not.Null);
//        });
//    }

//    [Test]
//    public void CancelSale_WhenCompleted_ShouldThrow()
//    {
//        // Arrange
//        const int completedSaleId = 1;

//        // Act & Assert
//        Assert.Throws<SqlException>(() =>
//            UnitOfWork.SaleRepository.CancelSale(
//                completedSaleId,
//                1,
//                "Trying to cancel completed sale"));
//    }

//    [Test]
//    public void CancelSale_WhenAlreadyCancelled_ShouldThrow()
//    {
//        // Arrange
//        const int cancelledSaleId = 3;

//        // Act & Assert
//        Assert.Throws<SqlException>(() =>
//            UnitOfWork.SaleRepository.CancelSale(
//                cancelledSaleId,
//                1,
//                "Trying to cancel again"));
//    }

//    [Test]
//    public void CompleteSale_ShouldCompleteDraftSale()
//    {
//        // Arrange
//        var sale = SalesTestDataFactory.CreateSale(
//            employeeId: 3,
//            status: SaleStatus.Draft);

//        var saleId =
//            UnitOfWork.SaleRepository.Insert(sale);

//        var saleItem = new SaleItemDTO
//        {
//            SaleId = saleId,
//            ProductId = 1,
//            Quantity = 1,
//            UnitPrice = 1200,
//            DiscountAmount = 0
//        };

//        UnitOfWork.SaleItemRepository.Insert(saleItem);

//        // Act
//        UnitOfWork.SaleRepository.CompleteSale(saleId);

//        // Assert
//        var completedSale =
//            UnitOfWork.SaleRepository.GetById(saleId);

//        Assert.That(completedSale, Is.Not.Null);
//        Assert.That(
//            completedSale!.Status,
//            Is.EqualTo(SaleStatus.Completed));
//    }

//    [Test]
//    public void CompleteSale_ShouldCreateStockMovement()
//    {
//        // Arrange
//        var sale = SalesTestDataFactory.CreateSale(
//            employeeId: 3,
//            status: SaleStatus.Draft);

//        var saleId =
//            UnitOfWork.SaleRepository.Insert(sale);

//        var saleItem = new SaleItemDTO
//        {
//            SaleId = saleId,
//            ProductId = 1,
//            Quantity = 1,
//            UnitPrice = 1200,
//            DiscountAmount = 0
//        };

//        var saleItemId =
//            UnitOfWork.SaleItemRepository.Insert(saleItem);

//        var movementsBefore =
//            UnitOfWork.StockMovementRepository
//                .GetByProductId(1)
//                .ToList();

//        // Act
//        UnitOfWork.SaleRepository.CompleteSale(saleId);

//        // Assert
//        var movementsAfter =
//            UnitOfWork.StockMovementRepository
//                .GetByProductId(1)
//                .ToList();

//        Assert.That(
//            movementsAfter.Count,
//            Is.EqualTo(movementsBefore.Count + 1));

//        var movement = movementsAfter
//            .OrderBy(m => m.Id)
//            .Last();

//        Assert.Multiple(() =>
//        {
//            Assert.That(movement.ProductId, Is.EqualTo(1));
//            Assert.That(movement.MovementType, Is.EqualTo(0));
//            Assert.That(movement.QuantityChange, Is.EqualTo(-1));
//            Assert.That(movement.SaleItemId, Is.EqualTo(saleItemId));
//            Assert.That(movement.ChangedByEmployeeId, Is.EqualTo(3));
//        });
//    }

//    [Test]
//    public void CompleteSale_WhenSaleHasNoItems_ShouldThrow()
//    {
//        // Arrange
//        var sale = SalesTestDataFactory.CreateSale(
//            employeeId: 3,
//            status: SaleStatus.Draft);

//        var saleId =
//            UnitOfWork.SaleRepository.Insert(sale);

//        // Act & Assert
//        Assert.Throws<SqlException>(() =>
//            UnitOfWork.SaleRepository.CompleteSale(saleId));

//        var unchangedSale =
//            UnitOfWork.SaleRepository.GetById(saleId);

//        Assert.That(
//            unchangedSale!.Status,
//            Is.EqualTo(SaleStatus.Draft));
//    }

//    [Test]
//    public void CompleteSale_WhenSaleIsAlreadyCompleted_ShouldThrow()
//    {
//        // Arrange
//        const int completedSaleId = 1;

//        // Act & Assert
//        Assert.Throws<SqlException>(() =>
//            UnitOfWork.SaleRepository
//                .CompleteSale(completedSaleId));
//    }

//    [Test]
//    public void CompleteSale_WhenSaleIsCancelled_ShouldThrow()
//    {
//        // Arrange
//        const int cancelledSaleId = 3;

//        // Act & Assert
//        Assert.Throws<SqlException>(() =>
//            UnitOfWork.SaleRepository
//                .CompleteSale(cancelledSaleId));
//    }

//    [Test]
//    public void CompleteSale_WhenStockIsInsufficient_ShouldThrow()
//    {
//        // Arrange
//        var sale = SalesTestDataFactory.CreateSale(
//            employeeId: 3,
//            status: SaleStatus.Draft);

//        var saleId =
//            UnitOfWork.SaleRepository.Insert(sale);

//        var saleItem = new SaleItemDTO
//        {
//            SaleId = saleId,
//            ProductId = 1,
//            Quantity = 100000,
//            UnitPrice = 1200,
//            DiscountAmount = 0
//        };

//        UnitOfWork.SaleItemRepository.Insert(saleItem);

//        var movementCountBefore =
//            UnitOfWork.StockMovementRepository
//                .GetByProductId(1)
//                .Count();

//        // Act & Assert
//        Assert.Throws<SqlException>(() =>
//            UnitOfWork.SaleRepository.CompleteSale(saleId));

//        var saleAfter =
//            UnitOfWork.SaleRepository.GetById(saleId);

//        var movementCountAfter =
//            UnitOfWork.StockMovementRepository
//                .GetByProductId(1)
//                .Count();

//        Assert.Multiple(() =>
//        {
//            Assert.That(
//                saleAfter!.Status,
//                Is.EqualTo(SaleStatus.Draft));

//            Assert.That(
//                movementCountAfter,
//                Is.EqualTo(movementCountBefore));
//        });
//    }
//}