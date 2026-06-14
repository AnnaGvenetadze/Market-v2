using Market.DTO;
using Market.Repositories;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class SaleItemRepositoryTests : BaseRepositoryTests
{
    private const string ConnectionString // = "Server=localhost;Database=Market.Database;Trusted_Connection=True;TrustServerCertificate=True;";
    = "Server=localhost;Database=MarketDB_Test;Trusted_Connection=True;TrustServerCertificate=True;";
    private SqlConnection _connection;
    private SaleItemRepository _repository;

    private const string ClearSaleItemDatabaseScript = @"
        DELETE FROM SaleItems;
        DELETE FROM Sales;

        DELETE FROM ProductAttributeValues;
        DELETE FROM Products;

        DELETE FROM CategoryAttributes;
        DELETE FROM Categories;

        DELETE FROM InventoryManagerDetails;
        DELETE FROM EmployeeRoles;
        DELETE FROM Employees;

        DELETE FROM Clients;
        DELETE FROM ClientTypes;

        DELETE FROM Accounts;

        DBCC CHECKIDENT ('SaleItems', RESEED, 0);
        DBCC CHECKIDENT ('Sales', RESEED, 0);
        DBCC CHECKIDENT ('Products', RESEED, 0);
        DBCC CHECKIDENT ('Categories', RESEED, 0);
        DBCC CHECKIDENT ('Employees', RESEED, 0);
        DBCC CHECKIDENT ('Clients', RESEED, 0);
        DBCC CHECKIDENT ('ClientTypes', RESEED, 0);
        DBCC CHECKIDENT ('Accounts', RESEED, 0);
    ";

    private const string SeedSaleItemDatabaseScript = @"
        INSERT INTO Accounts
        (
            Username,
            PasswordHash,
            Email,
            FirstName,
            LastName,
            AccountType,
            IsDeleted,
            CreateDate
        )
        VALUES
        (
            'employeeuser1',
            'test123',
            'employee1@test.com',
            'Employee',
            'One',
            1,
            0,
            GETDATE()
        ),
        (
            'employeeuser2',
            'test456',
            'employee2@test.com',
            'Employee',
            'Two',
            1,
            0,
            GETDATE()
        );

        INSERT INTO Employees
        (
            AccountId,
            ManagerEmployeeId,
            FirstName,
            LastName,
            PhoneNumber,
            ContactEmail,
            EmployeeCode,
            HireDate,
            IsDeleted,
            CreateDate
        )
        VALUES
        (
            1,
            NULL,
            'Giorgi',
            'Employee',
            '555111222',
            'giorgi.employee@test.com',
            'EMP001',
            GETDATE(),
            0,
            GETDATE()
        ),
        (
            2,
            NULL,
            'Nika',
            'Employee',
            '555333444',
            'nika.employee@test.com',
            'EMP002',
            GETDATE(),
            0,
            GETDATE()
        );

        INSERT INTO Categories
        (
            ParentId,
            CategoryName,
            Description,
            IsDeleted,
            CreatedDate
        )
        VALUES
        (
            NULL,
            'Test Category 1',
            'Test category description 1',
            0,
            GETDATE()
        ),
        (
            NULL,
            'Test Category 2',
            'Test category description 2',
            0,
            GETDATE()
        ),
        (
            NULL,
            'Test Category 3',
            'Test category description 3',
            0,
            GETDATE()
        );

        INSERT INTO Products
        (
            CategoryId,
            ProductName,
            Price,
            IsDeleted,
            CreatedDate
        )
        VALUES
        (
            1,
            'Product One',
            100.00,
            0,
            GETDATE()
        ),
        (
            2,
            'Product Two',
            200.00,
            0,
            GETDATE()
        ),
        (
            3,
            'Product Three',
            300.00,
            0,
            GETDATE()
        );

        INSERT INTO Sales
        (
            CreatedEmployeeId,
            CancelledByEmployeeId,
            Status,
            CreatedDate,
            CancelledDate,
            CancelReason
        )
        VALUES
        (
            1,
            NULL,
            0,
            GETDATE(),
            NULL,
            NULL
        ),
        (
            1,
            NULL,
            0,
            GETDATE(),
            NULL,
            NULL
        ),
        (
            2,
            NULL,
            0,
            GETDATE(),
            NULL,
            NULL
        );

        INSERT INTO SaleItems
        (
            SaleId,
            ProductId,
            Quantity,
            UnitPrice,
            DiscountAmount
        )
        VALUES
        (
            1,
            1,
            2,
            100.00,
            0
        ),
        (
            2,
            2,
            3,
            200.00,
            0
        );
    ";

    public static void ClearSaleItemDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        using var command = new SqlCommand(ClearSaleItemDatabaseScript, connection);
        command.ExecuteNonQuery();
    }

    public static void SeedSaleItemDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        using var command = new SqlCommand(SeedSaleItemDatabaseScript, connection);
        command.ExecuteNonQuery();
    }

    [SetUp]
    public void Setup()
    {
        ClearSaleItemDatabase();
        SeedSaleItemDatabase();

        _connection = new SqlConnection(ConnectionString);
        _repository = new SaleItemRepository(_connection);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Dispose();
        _repository.Dispose();
    }

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

        var newId = _repository.Insert(saleItem);
        var inserted = _repository.GetById(newId);

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

        Assert.Throws<SqlException>(() => _repository.Insert(saleItem));
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

        var newId = _repository.Insert(saleItem);

        // Act

        _repository.Delete(newId);

        // Assert

        Assert.Throws<InvalidOperationException>(() => _repository.GetById(newId));
    }

    [Test]
    public void updateTest_ShouldModifyExistingItem()
    {
        // Arrange
        var saleItem = _repository.GetById(UpdateTestId);

        Assert.That(saleItem, Is.Not.Null);

        saleItem!.Quantity = 10;
        saleItem.UnitPrice = 150;
        saleItem.DiscountAmount = 5;

        // Act
        _repository.Update(saleItem);

        var updated = _repository.GetById(UpdateTestId);

        // Assert
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Quantity, Is.EqualTo(10));
        Assert.That(updated.UnitPrice, Is.EqualTo(150));
    }
}