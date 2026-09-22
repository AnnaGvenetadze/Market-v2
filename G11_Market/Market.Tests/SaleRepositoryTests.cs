using Market.DTO;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class SaleRepositoryTests : BaseRepositoryTests
{
    private const int FirstEmployeeId = 1;
    private const int SecondEmployeeId = 2;

    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = FirstEmployeeId,
            Status = 0
        };

        // Act
        var insertedId = UnitOfWork.SaleRepository.AssignAttribute(sale);
        var insertedSale = UnitOfWork.SaleRepository.GetById(insertedId);

        // Assert
        Assert.That(insertedSale, Is.Not.Null);
        Assert.That(insertedSale.Id, Is.EqualTo(insertedId));
        Assert.That(insertedSale.CreatedEmployeeId, Is.EqualTo(FirstEmployeeId));
        Assert.That(insertedSale.Status, Is.EqualTo(0));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var saleToUpdate = UnitOfWork.SaleRepository.GetById(UpdateTestId);

        saleToUpdate!.Status = 1;

        // Act
        UnitOfWork.SaleRepository.Update(saleToUpdate);

        var updatedSale = UnitOfWork.SaleRepository.GetById(UpdateTestId);

        // Assert
        Assert.That(updatedSale, Is.Not.Null);
        Assert.That(updatedSale.Status, Is.EqualTo(1));
    }

    [Test]
    public void CancelTest_ShouldCancelValidSale()
    {
        // Act
        UnitOfWork.SaleRepository.Cancel(
            DeleteTestId,
            SecondEmployeeId,
            "Cancelled in test");

        var cancelledSale = UnitOfWork.SaleRepository.GetById(DeleteTestId);

        // Assert
        Assert.That(cancelledSale, Is.Not.Null);
        Assert.That(cancelledSale!.Status, Is.EqualTo(2));
        Assert.That(cancelledSale.CancelledByEmployeeId, Is.EqualTo(SecondEmployeeId));
        Assert.That(cancelledSale.CancelReason, Is.EqualTo("Cancelled in test"));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidEmployeeId()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = -9,
            Status = 0
        };

        // Act & Assert
        Assert.Throws<SqlException>(
            () => UnitOfWork.SaleRepository.AssignAttribute(sale));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidStatus()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = FirstEmployeeId,
            Status = 3
        };

        // Act & Assert
        Assert.Throws<SqlException>(
            () => UnitOfWork.SaleRepository.AssignAttribute(sale));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyCancelReason()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = FirstEmployeeId,
            Status = 2,
            CancelledByEmployeeId = SecondEmployeeId,
            CancelledDate = DateTime.Now,
            CancelReason = "     "
        };

        // Act & Assert
        Assert.Throws<SqlException>(
            () => UnitOfWork.SaleRepository.AssignAttribute(sale));
    }

    [Test]
    public void UpdateTest_ShouldAllowValidCancellation()
    {
        // Arrange
        var saleToCancel = UnitOfWork.SaleRepository.GetById(UpdateTestId);

        saleToCancel!.Status = 2;
        saleToCancel.CancelledByEmployeeId = SecondEmployeeId;
        saleToCancel.CancelledDate = DateTime.Now;
        saleToCancel.CancelReason = "Customer does not want it";

        // Act
        UnitOfWork.SaleRepository.Update(saleToCancel);

        var updatedSale = UnitOfWork.SaleRepository.GetById(UpdateTestId);

        // Assert
        Assert.That(updatedSale, Is.Not.Null);
        Assert.That(updatedSale!.Status, Is.EqualTo(2));
        Assert.That(updatedSale.CancelledByEmployeeId, Is.EqualTo(SecondEmployeeId));
        Assert.That(updatedSale.CancelReason, Is.EqualTo("Customer does not want it"));
    }

    [Test]
    public void GetSalesByEmployee_ShouldReturnCorrectData()
    {
        // Act
        var employeeSales = UnitOfWork.SaleRepository
            .GetSalesByEmployee(FirstEmployeeId)
            .ToList();

        // Assert
        Assert.That(employeeSales, Is.Not.Empty);
        Assert.That(employeeSales.All(x => x.CreatedEmployeeId == FirstEmployeeId), Is.True);
    }

    [Test]
    public void GetCompletedSales_ShouldReturnCorrectData()
    {
        // Act
        var completedSales = UnitOfWork.SaleRepository
            .GetCompletedSales()
            .ToList();

        // Assert
        Assert.That(completedSales, Is.Not.Empty);
        Assert.That(completedSales.All(x => x.Status == 1), Is.True);
        Assert.That(completedSales.Any(x => x.Id == UpdateTestId), Is.True);
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => UnitOfWork.SaleRepository.GetById(null!));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => UnitOfWork.SaleRepository.Update(null!));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => UnitOfWork.SaleRepository.Delete(null!));
    }

    [Test]
    public void GetAll_ShouldReturnSeededRecords()
    {
        // Act
        var allSales = UnitOfWork.SaleRepository.GetAll().ToList();

        // Assert
        Assert.That(allSales.Count, Is.AtLeast(3));
        Assert.That(allSales.Any(x => x.Id == 1), Is.True);
        Assert.That(allSales.Any(x => x.Id == 2), Is.True);
        Assert.That(allSales.Any(x => x.Id == 3), Is.True);
    }
}