using Market.DTO;
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
        Assert.That(updated!.Quantity, Is.EqualTo(newQuantity));
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
    public void UpdateQuantity_WhenQuantityIsInvalid_ShouldThrow()
    {
        // Arrange
        const int saleItemId = 3;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.SaleItemRepository.UpdateQuantity(
                saleItemId,
                0));
    }

    [Test]
    public void Insert_WhenSaleIsDraft_ShouldInsertSaleItem()
    {
        // Arrange
        var saleItem = new SaleItemDTO
        {
            SaleId = 2,
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
            Assert.That(inserted!.SaleId, Is.EqualTo(2));
            Assert.That(inserted.ProductId, Is.EqualTo(1));
            Assert.That(inserted.Quantity, Is.EqualTo(2));
            Assert.That(inserted.UnitPrice, Is.EqualTo(1200));
            Assert.That(inserted.DiscountAmount, Is.EqualTo(0));
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
    public void Delete_WhenSaleIsDraft_ShouldDeleteSaleItem()
    {
        // Arrange
        const int saleItemId = 3;

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
}