using Market.DTO;
using Market.DTO.Enums;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class SaleItemRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void GetBySaleId_ShouldReturnOnlyItemsOfRequestedSale()
    {
        // Arrange
        const int saleId = 1;

        // Act
        var saleItems = UnitOfWork.SaleItemRepository
            .GetBySaleId(saleId)
            .ToList();

        // Assert
        Assert.That(saleItems, Has.Count.EqualTo(2));

        Assert.That(
            saleItems.All(item => item.SaleId == saleId),
            Is.True);
    }

    [Test]
    public void GetBySaleId_WhenSaleHasNoItems_ShouldReturnEmpty()
    {
        // Arrange
        var sale = SalesTestDataFactory.CreateSale(
            employeeId: 3,
            status: SaleStatus.Draft);

        var saleId =
            UnitOfWork.SaleRepository.Insert(sale);

        // Act
        var saleItems = UnitOfWork.SaleItemRepository
            .GetBySaleId(saleId)
            .ToList();

        // Assert
        Assert.That(saleItems, Is.Empty);
    }

    [Test]
    public void UpdateQuantity_ShouldUpdateDraftSaleItem()
    {
        // Arrange
        const int saleItemId = 3;
        const int newQuantity = 5;

        // Act
        UnitOfWork.SaleItemRepository.UpdateQuantity(
            saleItemId,
            newQuantity);

        // Assert
        var updated =
            UnitOfWork.SaleItemRepository.GetById(saleItemId);

        Assert.That(updated, Is.Not.Null);
        Assert.That(
            updated!.Quantity,
            Is.EqualTo(newQuantity));
    }

    [Test]
    public void UpdateQuantity_WhenSaleIsCompleted_ShouldThrow()
    {
        // Arrange
        const int saleItemId = 1;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleItemRepository.UpdateQuantity(
                saleItemId,
                5));
    }

    [Test]
    public void UpdateQuantity_WhenQuantityIsNegative_ShouldThrow()
    {
        // Arrange
        const int saleItemId = 203;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.SaleItemRepository.UpdateQuantity(
                saleItemId,
                -1));
    }

    [Test]
    public void UpdateQuantity_WhenQuantityIsZero_ShouldThrow()
    {
        // Arrange
        const int saleItemId = 203;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.SaleItemRepository.UpdateQuantity(
                saleItemId,
                0));
    }

    [Test]
    public void Insert_WhenSaleIsDraft_ShouldInsertSaleItem()
    {
        // Arrange
        var sale = SalesTestDataFactory.CreateSale(
            employeeId: 3,
            status: SaleStatus.Draft);

        var saleId =
            UnitOfWork.SaleRepository.Insert(sale);

        var saleItem = new SaleItemDTO
        {
            SaleId = saleId,
            ProductId = 1,
            Quantity = 2,
            UnitPrice = 1200,
            DiscountAmount = 0
        };

        // Act
        var newId =
            UnitOfWork.SaleItemRepository.Insert(saleItem);

        // Assert
        var inserted =
            UnitOfWork.SaleItemRepository.GetById(newId);

        Assert.That(inserted, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(
                inserted!.SaleId,
                Is.EqualTo(saleId));

            Assert.That(
                inserted.ProductId,
                Is.EqualTo(1));

            Assert.That(
                inserted.Quantity,
                Is.EqualTo(2));

            Assert.That(
                inserted.UnitPrice,
                Is.EqualTo(1200));

            Assert.That(
                inserted.DiscountAmount,
                Is.EqualTo(0));
        });
    }

    [Test]
    public void Insert_WhenSaleIsCompleted_ShouldThrow()
    {
        // Arrange
        var saleItem = new SaleItemDTO
        {
            SaleId = 1,
            ProductId = 3,
            Quantity = 1,
            UnitPrice = 3000,
            DiscountAmount = 0
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleItemRepository.Insert(saleItem));
    }

    [Test]
    public void Insert_WhenSaleIsCancelled_ShouldThrow()
    {
        // Arrange
        var saleItem = new SaleItemDTO
        {
            SaleId = 3,
            ProductId = 1,
            Quantity = 1,
            UnitPrice = 1200,
            DiscountAmount = 0
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleItemRepository.Insert(saleItem));
    }

    [Test]
    public void Insert_WhenProductAlreadyExistsInSale_ShouldThrow()
    {
        // Arrange
        const int draftSaleId = 2;

        var saleItem = new SaleItemDTO
        {
            SaleId = draftSaleId,
            ProductId = 3,
            Quantity = 1,
            UnitPrice = 3000,
            DiscountAmount = 0
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleItemRepository.Insert(saleItem));
    }

    [Test]
    public void Insert_WhenQuantityIsInvalid_ShouldThrow()
    {
        // Arrange
        var sale = SalesTestDataFactory.CreateSale(
            employeeId: 3,
            status: SaleStatus.Draft);

        var saleId =
            UnitOfWork.SaleRepository.Insert(sale);

        var saleItem = new SaleItemDTO
        {
            SaleId = saleId,
            ProductId = 1,
            Quantity = 0,
            UnitPrice = 1200,
            DiscountAmount = 0
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleItemRepository.Insert(saleItem));
    }

    [Test]
    public void Delete_WhenSaleIsDraft_ShouldDeleteSaleItem()
    {
        // Arrange
        var sale = SalesTestDataFactory.CreateSale(
            employeeId: 3,
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

        var saleItemId =
            UnitOfWork.SaleItemRepository.Insert(saleItem);

        // Act
        UnitOfWork.SaleItemRepository.Delete(saleItemId);

        // Assert
        var deleted =
            UnitOfWork.SaleItemRepository.GetById(saleItemId);

        Assert.That(deleted, Is.Null);
    }

    [Test]
    public void Delete_WhenSaleIsCompleted_ShouldThrow()
    {
        // Arrange
        const int saleItemId = 1;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleItemRepository.Delete(saleItemId));
    }

    [Test]
    public void Delete_WhenSaleIsCancelled_ShouldThrow()
    {
        // Arrange
        var sale = SalesTestDataFactory.CreateSale(
            employeeId: 3,
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

        var saleItemId =
            UnitOfWork.SaleItemRepository.Insert(saleItem);

        UnitOfWork.SaleRepository.CancelSale(
            saleId,
            1,
            "Test cancellation");

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleItemRepository.Delete(saleItemId));
    }
}