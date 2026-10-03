using Market.DTO.Enums;
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
    public void GetSalesByStatus_WhenStatusIsInvalid_ShouldThrow()
    {
        var invalidStatus = (SaleStatus)99;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.SaleRepository
                .GetSalesByStatus(invalidStatus)
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
    public void GetSalesByDateRange_WhenRangeInvalid_ShouldThrow()
    {
        // Arrange
        var dateFrom = DateTime.Now;
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
        var dateFrom = DateTime.Now.AddYears(-10);
        var dateTo = DateTime.Now.AddYears(-9);

        var sales = UnitOfWork.SaleRepository
            .GetSalesByDateRange(dateFrom, dateTo)
            .ToList();

        Assert.That(sales, Is.Empty);
    }

    [Test]
    public void CancelSale_ShouldCancelDraftSale()
    {
        // Arrange
        const int saleId = 2;
        const int employeeId = 1;
        const string cancelReason = "Customer did not return";

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
    public void CancelSale_WhenCompleted_ShouldThrow()
    {
        // Arrange
        const int completedSaleId = 1;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleRepository.CancelSale(
                completedSaleId,
                1,
                "Trying to cancel completed sale"));
    }

    [Test]
    public void CancelSale_WhenAlreadyCancelled_ShouldThrow()
    {
        // Arrange
        const int cancelledSaleId = 3;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleRepository.CancelSale(
                cancelledSaleId,
                1,
                "Trying to cancel again"));
    }
}