using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class InventoryManagerDetailsRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void InsertTest_ShouldNotInsertDuplicateEmployeeId()
    {
        // Arrange
        var inventoryManager = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: UpdateTestId);

        // Act & Assert
        Assert.Throws<SqlException>(
            () => UnitOfWork.InventoryManagerDetailsRepository.Insert(inventoryManager));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var inventoryManagerToUpdate = UnitOfWork.InventoryManagerDetailsRepository
            .GetById(UpdateTestId);

        inventoryManagerToUpdate!.StockAdjustmentLimit = 1500;
        inventoryManagerToUpdate.CanApproveStockCorrection = true;
        inventoryManagerToUpdate.CanApproveNegativeStock = true;

        // Act
        UnitOfWork.InventoryManagerDetailsRepository.Update(inventoryManagerToUpdate);

        var updatedInventoryManager = UnitOfWork.InventoryManagerDetailsRepository
            .GetById(UpdateTestId);

        // Assert
        Assert.That(updatedInventoryManager, Is.Not.Null);
        Assert.That(updatedInventoryManager.Id, Is.EqualTo(UpdateTestId));
        Assert.That(updatedInventoryManager.StockAdjustmentLimit, Is.EqualTo(1500));
        Assert.That(updatedInventoryManager.CanApproveStockCorrection, Is.True);
        Assert.That(updatedInventoryManager.CanApproveNegativeStock, Is.True);
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Act
        UnitOfWork.InventoryManagerDetailsRepository.Delete(DeleteTestId);

        // Assert
        Assert.Throws<SqlException>(
            () => UnitOfWork.InventoryManagerDetailsRepository.GetById(DeleteTestId));
    }

    [Test]
    public void GetByEmployeeId_ShouldReturnCorrectInventoryManager()
    {
        // Act
        var result = UnitOfWork.InventoryManagerDetailsRepository
            .GetByEmployeeId(UpdateTestId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(UpdateTestId));
        Assert.That(result.StockAdjustmentLimit, Is.EqualTo(1000.00m));
    }

    [Test]
    public void GetAllActive_ShouldReturnSeededActiveInventoryManagers()
    {
        // Act
        var activeInventoryManagers = UnitOfWork.InventoryManagerDetailsRepository
            .GetAllActive()
            .ToList();

        // Assert
        Assert.That(activeInventoryManagers, Is.Not.Empty);
        Assert.That(activeInventoryManagers.Any(x => x.Id == 1), Is.True);
        Assert.That(activeInventoryManagers.Any(x => x.Id == 2), Is.True);
        Assert.That(activeInventoryManagers.Any(x => x.Id == 3), Is.True);
        Assert.That(activeInventoryManagers.All(x => !x.IsDeleted), Is.True);
    }

    [Test]
    public void GetAllActive_ShouldExcludeDeletedInventoryManagers()
    {
        // Arrange
        UnitOfWork.InventoryManagerDetailsRepository.Delete(DeleteTestId);

        // Act
        var activeInventoryManagers = UnitOfWork.InventoryManagerDetailsRepository
            .GetAllActive()
            .ToList();

        // Assert
        Assert.That(activeInventoryManagers.Any(x => x.Id == UpdateTestId), Is.True);
        Assert.That(activeInventoryManagers.Any(x => x.Id == DeleteTestId), Is.False);
        Assert.That(activeInventoryManagers.All(x => !x.IsDeleted), Is.True);
    }

    [Test]
    public void GetManagersWhoCanApproveStockCorrection_ShouldReturnMatchingManagers()
    {
        // Act
        var results = UnitOfWork.InventoryManagerDetailsRepository
            .GetManagersWhoCanApproveStockCorrection()
            .ToList();

        // Assert
        Assert.That(results, Is.Not.Empty);
        Assert.That(results.Any(x => x.Id == 1), Is.True);
        Assert.That(results.Any(x => x.Id == 2), Is.True);
        Assert.That(results.Any(x => x.Id == 3), Is.False);
        Assert.That(results.All(x => x.CanApproveStockCorrection), Is.True);
    }

    [Test]
    public void GetManagersWhoCanApproveNegativeStock_ShouldReturnMatchingManagers()
    {
        // Act
        var results = UnitOfWork.InventoryManagerDetailsRepository
            .GetManagersWhoCanApproveNegativeStock()
            .ToList();

        // Assert
        Assert.That(results, Is.Not.Empty);
        Assert.That(results.Any(x => x.Id == 1), Is.True);
        Assert.That(results.Any(x => x.Id == 2), Is.False);
        Assert.That(results.Any(x => x.Id == 3), Is.False);
        Assert.That(results.All(x => x.CanApproveNegativeStock), Is.True);
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidEmployeeId()
    {
        // Arrange
        var inventoryManager = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: -1);

        // Act & Assert
        Assert.Throws<SqlException>(
            () => UnitOfWork.InventoryManagerDetailsRepository.Insert(inventoryManager));
    }

    [Test]
    public void InsertTest_ShouldNotInsertNegativeStockAdjustmentLimit()
    {
        // Arrange
        var inventoryManager = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: UpdateTestId,
            stockAdjustmentLimit: -1);

        // Act & Assert
        Assert.Throws<SqlException>(
            () => UnitOfWork.InventoryManagerDetailsRepository.Insert(inventoryManager));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateNegativeStockAdjustmentLimit()
    {
        // Arrange
        var inventoryManagerToUpdate = UnitOfWork.InventoryManagerDetailsRepository
            .GetById(UpdateTestId);

        inventoryManagerToUpdate!.StockAdjustmentLimit = -1;

        // Act & Assert
        Assert.Throws<SqlException>(
            () => UnitOfWork.InventoryManagerDetailsRepository.Update(inventoryManagerToUpdate));
    }
}