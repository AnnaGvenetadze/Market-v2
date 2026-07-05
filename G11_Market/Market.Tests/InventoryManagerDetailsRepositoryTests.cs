using Market.DTO;
using Market.Services.Interfaces.Repositories;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class InventoryManagerDetailsRepositoryTests : BaseRepositoryTests
{
    private SqlConnection _connection;
    private IAccountRepository _accountRepository;
    private IEmployeeRepository _employeeRepository;
    private IInventoryManagerDetailsRepository _inventoryManagerDetailsRepository;

    [SetUp]
    public void Setup()
    {
        _connection = new SqlConnection(ConnectionString);
        _accountRepository = UnitOfWork.AccountRepository;
        _employeeRepository = UnitOfWork.EmployeeRepository;
        _inventoryManagerDetailsRepository = UnitOfWork.InventoryManagerDetailsRepository;
    }


    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var employeeId = CreateEmployee();

        var inventoryManager = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId,
            stockAdjustmentLimit: 500,
            canApproveStockCorrection: true,
            canApproveNegativeStock: false);

        // Act
        var insertedId = _inventoryManagerDetailsRepository
            .Insert(inventoryManager);
        var insertedInventoryManager = _inventoryManagerDetailsRepository
            .GetById(insertedId);

        // Assert
        Assert.That(insertedInventoryManager, Is.Not.Null);
        Assert.That(insertedInventoryManager.Id, Is.EqualTo(employeeId));
        Assert.That(insertedInventoryManager.StockAdjustmentLimit, Is.EqualTo(500));
        Assert.That(insertedInventoryManager.CanApproveStockCorrection, Is.True);
        Assert.That(insertedInventoryManager.CanApproveNegativeStock, Is.False);
    }


    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var employeeId = CreateEmployee();

        var inventoryManager = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId,
            stockAdjustmentLimit: 500,
            canApproveStockCorrection: false,
            canApproveNegativeStock: false);

        var insertedId = _inventoryManagerDetailsRepository.Insert(inventoryManager);

        var inventoryManagerToUpdate = _inventoryManagerDetailsRepository
            .GetById(insertedId);
        inventoryManagerToUpdate!.StockAdjustmentLimit = 1500;
        inventoryManagerToUpdate.CanApproveStockCorrection = true;
        inventoryManagerToUpdate.CanApproveNegativeStock = true;

        // Act
        _inventoryManagerDetailsRepository.Update(inventoryManagerToUpdate);

        var updatedInventoryManager = _inventoryManagerDetailsRepository
            .GetById(insertedId);

        // Assert
        Assert.That(updatedInventoryManager, Is.Not.Null);
        Assert.That(updatedInventoryManager.StockAdjustmentLimit, Is.EqualTo(1500));
        Assert.That(updatedInventoryManager.CanApproveStockCorrection, Is.True);
        Assert.That(updatedInventoryManager.CanApproveNegativeStock, Is.True);
    }


    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var employeeId = CreateEmployee();

        var inventoryManager = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId);

        var insertedId = _inventoryManagerDetailsRepository.Insert(inventoryManager);

        // Act
        _inventoryManagerDetailsRepository.Delete(insertedId);

        // Assert
        Assert.Throws<SqlException>(
            () => _inventoryManagerDetailsRepository.GetById(insertedId));
    }


    [Test]
    public void GetByEmployeeId_ShouldReturnCorrectInventoryManager()
    {
        // Arrange
        var employeeId = CreateEmployee();

        var inventoryManager = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId,
            stockAdjustmentLimit: 700);

        _inventoryManagerDetailsRepository.Insert(inventoryManager);

        // Act
        var result = _inventoryManagerDetailsRepository.GetByEmployeeId(employeeId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(employeeId));
        Assert.That(result.StockAdjustmentLimit, Is.EqualTo(700));
    }


    [Test]
    public void GetAllActive_ShouldExcludeDeletedInventoryManagers()
    {
        // Arrange
        var activeEmployeeId = CreateEmployee();
        var deletedEmployeeId = CreateEmployee();

        var activeInventoryManager = InventoryManagerTestDataFactory
            .CreateInventoryManager(employeeId: activeEmployeeId);

        var deletedInventoryManager = InventoryManagerTestDataFactory
            .CreateInventoryManager(employeeId: deletedEmployeeId);

        var activeId = _inventoryManagerDetailsRepository.Insert(activeInventoryManager);
        var deletedId = _inventoryManagerDetailsRepository.Insert(deletedInventoryManager);

        _inventoryManagerDetailsRepository.Delete(deletedId);

        // Act
        var activeInventoryManagers = _inventoryManagerDetailsRepository.GetAllActive().ToList();

        // Assert
        Assert.That(activeInventoryManagers.Any(x => x.Id == activeId), Is.True);
        Assert.That(activeInventoryManagers.Any(x => x.Id == deletedId), Is.False);
        Assert.That(activeInventoryManagers.All(x => !x.IsDeleted), Is.True);
    }


    [Test]
    public void GetManagersWhoCanApproveStockCorrection_ShouldReturnMatchingManagers()
    {
        // Arrange
        var employeeId1 = CreateEmployee();
        var employeeId2 = CreateEmployee();

        var manager1 = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId1,
            canApproveStockCorrection: true);

        var manager2 = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId2,
            canApproveStockCorrection: false);

        _inventoryManagerDetailsRepository.Insert(manager1);
        _inventoryManagerDetailsRepository.Insert(manager2);

        // Act
        var results = _inventoryManagerDetailsRepository
            .GetManagersWhoCanApproveStockCorrection()
            .ToList();

        // Assert
        Assert.That(results.Any(x => x.Id == employeeId1), Is.True);
        Assert.That(results.Any(x => x.Id == employeeId2), Is.False);
        Assert.That(results.All(x => x.CanApproveStockCorrection), Is.True);
    }


    [Test]
    public void GetManagersWhoCanApproveNegativeStock_ShouldReturnMatchingManagers()
    {
        // Arrange
        var employeeId1 = CreateEmployee();
        var employeeId2 = CreateEmployee();

        var manager1 = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId1,
            canApproveNegativeStock: true);

        var manager2 = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId2,
            canApproveNegativeStock: false);

        _inventoryManagerDetailsRepository.Insert(manager1);
        _inventoryManagerDetailsRepository.Insert(manager2);

        // Act
        var results = _inventoryManagerDetailsRepository
            .GetManagersWhoCanApproveNegativeStock()
            .ToList();

        // Assert
        Assert.That(results.Any(x => x.Id == employeeId1), Is.True);
        Assert.That(results.Any(x => x.Id == employeeId2), Is.False);
        Assert.That(results.All(x => x.CanApproveNegativeStock), Is.True);
    }


    [Test]
    public void InsertTest_ShouldNotInsertDuplicateEmployeeId()
    {
        // Arrange
        var employeeId = CreateEmployee();

        var manager1 = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId);

        _inventoryManagerDetailsRepository.Insert(manager1);

        var manager2 = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId);

        // Act & Assert
        Assert.Throws<SqlException>(
            () => _inventoryManagerDetailsRepository.Insert(manager2));
    }


    [Test]
    public void InsertTest_ShouldNotInsertInvalidEmployeeId()
    {
        // Arrange
        var inventoryManager = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: -1);

        // Act & Assert
        Assert.Throws<SqlException>(
            () => _inventoryManagerDetailsRepository.Insert(inventoryManager));
    }


    [Test]
    public void InsertTest_ShouldNotInsertNegativeStockAdjustmentLimit()
    {
        // Arrange
        var employeeId = CreateEmployee();

        var inventoryManager = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId,
            stockAdjustmentLimit: -1);

        // Act & Assert
        Assert.Throws<SqlException>(
            () => _inventoryManagerDetailsRepository.Insert(inventoryManager));
    }


    [Test]
    public void UpdateTest_ShouldNotUpdateNegativeStockAdjustmentLimit()
    {
        // Arrange
        var employeeId = CreateEmployee();

        var inventoryManager = InventoryManagerTestDataFactory.CreateInventoryManager(
            employeeId: employeeId,
            stockAdjustmentLimit: 500);

        var insertedId = _inventoryManagerDetailsRepository.Insert(inventoryManager);

        var inventoryManagerToUpdate = _inventoryManagerDetailsRepository
            .GetById(insertedId);
        inventoryManagerToUpdate!.StockAdjustmentLimit = -1;

        // Act & Assert
        Assert.Throws<SqlException>(
            () => _inventoryManagerDetailsRepository.Update(inventoryManagerToUpdate));
    }


    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _inventoryManagerDetailsRepository.GetById(null!));
    }


    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _inventoryManagerDetailsRepository.Update(null!));
    }


    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _inventoryManagerDetailsRepository.Delete(null!));
    }


    [TearDown]
    public void TearDown()
    {
        _connection.Dispose();
    }


    private int CreateEmployee()
    {
        var account = new AccountDTO
        {
            Username = "User".AddGuid(),
            PasswordHash = "Password123",
            Email = "Email".AddGuid() + "@gmail.com",
            AccountType = 1,
            FirstName = "Test",
            LastName = "Employee"
        };
        var accountId = _accountRepository.Insert(account);

        var employee = EmployeeTestDataFactory.CreateEmployee(
            accountId: accountId);

        return _employeeRepository.Insert(employee);
    }
}