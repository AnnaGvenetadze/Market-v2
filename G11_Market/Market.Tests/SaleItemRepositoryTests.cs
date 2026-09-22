using Market.DTO;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class SaleItemRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange

        var saleItem = new SaleItemDTO
        {
            SaleId = 3,
            ProductId = 3,
            Quantity = 5,
            UnitPrice = 300,
            DiscountAmount = 0
        };

        // Act

        var newId = UnitOfWork.SaleItemRepository.AssignAttribute(saleItem);
        var inserted = UnitOfWork.SaleItemRepository.GetById(newId);

        // Assert

        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(inserted, Is.Not.Null);
        Assert.That(inserted!.SaleId, Is.EqualTo(saleItem.SaleId));
        Assert.That(inserted.ProductId, Is.EqualTo(saleItem.ProductId));
        Assert.That(inserted.Quantity, Is.EqualTo(saleItem.Quantity));
        Assert.That(inserted.UnitPrice, Is.EqualTo(saleItem.UnitPrice));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidData()
    {
        // Arrange

        var saleItem = new SaleItemDTO
        {
            SaleId = 0,
            ProductId = 0,
            Quantity = -1,
            UnitPrice = -10.0m
        };

        // Act and Assert

        Assert.Throws<SqlException>(() => UnitOfWork.SaleItemRepository.AssignAttribute(saleItem));
    }

    [Test]

    public void DeleteTest_ShouldRemoveItem()
    {
        // Arrange

        var saleItem = new SaleItemDTO
        {
            SaleId = 3,
            ProductId = 3,
            Quantity = 5,
            UnitPrice = 300,
            DiscountAmount = 0
        };

        var newId = UnitOfWork.SaleItemRepository.AssignAttribute(saleItem);

        // Act

        UnitOfWork.SaleItemRepository.Delete(newId);

        // Assert

        Assert.Throws<InvalidOperationException>(() => UnitOfWork.SaleItemRepository.GetById(newId));
    }

    [Test]
    public void updateTest_ShouldModifyExistingItem()
    {
        // Arrange
        var saleItem = UnitOfWork.SaleItemRepository.GetById(UpdateTestId);

        Assert.That(saleItem, Is.Not.Null);

        saleItem!.Quantity = 10;
        saleItem.UnitPrice = 150;
        saleItem.DiscountAmount = 5;

        // Act
        UnitOfWork.SaleItemRepository.Update(saleItem);

        var updated = UnitOfWork.SaleItemRepository.GetById(UpdateTestId);

        // Assert
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Quantity, Is.EqualTo(10));
        Assert.That(updated.UnitPrice, Is.EqualTo(150));
    }
}