using Dapper;
using Market.DTO;
using Market.Repositories;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class ClientRepositoryTests : BaseRepositoryTests
{
    private SqlConnection _connection;
    private ClientRepository _repository;

    private const string ClearClientDatabaseScript = @"
        DELETE FROM SaleItems;
        DELETE FROM Sales;

        DELETE FROM Clients;
        DELETE FROM Employees;

        DELETE FROM ClientTypes;
        DELETE FROM Accounts;

        DBCC CHECKIDENT ('SaleItems', RESEED, 0);
        DBCC CHECKIDENT ('Sales', RESEED, 0);

        DBCC CHECKIDENT ('Clients', RESEED, 0);
        DBCC CHECKIDENT ('Employees', RESEED, 0);

        DBCC CHECKIDENT ('ClientTypes', RESEED, 0);
        DBCC CHECKIDENT ('Accounts', RESEED, 0);
    ";

    private const string SeedClientDatabaseScript = @"
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
        'testuser1',
        'test123',
        'test@gmail.com',
        'test',
        'test',
        2,
        0,
        GETDATE()
    ),
    (
        'testuser2',
        'test456',
        'test2@gmail.com',
        'test',
        'test',
        2,
        0,
        GETDATE()
    ),
    (
        'testuser3',
        'test789',
        'test3@gmail.com',
        'test',
        'test',
        2,
        0,
        GETDATE()
    );

    INSERT INTO ClientTypes
    (
        Name,
        Description,
        IsDeleted,
        CreateDate
    )
    VALUES
    (
        'Individual',
        'Individual client type',
        0,
        GETDATE()
    ),
    (
        'Corporate',
        'Corporate client type',
        0,
        GETDATE()
    );

    INSERT INTO Clients
    (
        AccountId,
        ClientTypeId,
        FirstName,
        LastName,
        PhoneNumber,
        ContactEmail,
        IsDeleted,
        CreateDate
    )
    VALUES
    (
        1,
        1,
        'Giorgi',
        'Client',
        '555111222',
        'client1@test.com',
        0,
        GETDATE()
    ),
    (
        2,
        2,
        'Nika',
        'Client',
        '555333444',
        'client2@test.com',
        0,
        GETDATE()
    );
";

    public static void ClearClientDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        using var command = new SqlCommand(ClearClientDatabaseScript, connection);
        command.ExecuteNonQuery();
    }

    public static void SeedClientDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        using var command = new SqlCommand(SeedClientDatabaseScript, connection);
        command.ExecuteNonQuery();
    }

    [SetUp]
    public void Setup()
    {
        ClearClientDatabase();
        SeedClientDatabase();
        _connection = new SqlConnection(ConnectionString);
        _repository = new ClientRepository(_connection);
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

        var client = new ClientDTO
        {
            AccountId = 3,
            ClientTypeId = 1,
            FirstName = "Test",
            LastName = "Client",
            PhoneNumber = "555123456",
            ContactEmail = "test@gmail.com"
        };

        // Act

        var newId = _repository.Insert(client);
        var inserted = _repository.GetById(newId);

        // Assert

        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(inserted, Is.Not.Null);
        Assert.That(inserted!.FirstName, Is.EqualTo(client.FirstName));
        Assert.That(inserted.LastName, Is.EqualTo(client.LastName));
        Assert.That(inserted.AccountId, Is.EqualTo(client.AccountId));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidData()
    {
        // Arrange

        var client = new ClientDTO
        {
            AccountId = 3,
            ClientTypeId = 1,
            FirstName = "",
            LastName = "Test",
            PhoneNumber = "abc123",
            ContactEmail = "123"
        };

        // Act & Assert

        Assert.Throws<SqlException>(() => _repository.Insert(client));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange

        var client = _repository.GetById(UpdateTestId);

        client!.FirstName = "Updated";
        client.LastName = "Client";
        client.PhoneNumber = "555999888";
        client.ContactEmail = "updated@gmail.com";

        // Act

        _repository.Update(client);
        var updated = _repository.GetById(UpdateTestId);

        // Assert

        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.FirstName, Is.EqualTo("Updated"));
        Assert.That(updated.PhoneNumber, Is.EqualTo("555999888"));
        Assert.That(updated.ContactEmail, Is.EqualTo("updated@gmail.com"));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Act
        _repository.Delete(DeleteTestId);

        // Assert
        var exception = Assert.Throws<SqlException>(() => _repository.GetById(DeleteTestId));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Number, Is.EqualTo(50034));
    }
}