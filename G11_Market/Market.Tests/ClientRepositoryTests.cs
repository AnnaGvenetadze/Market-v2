using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Market.Tests;

[TestFixture]
public class ClientRepositoryTests : BaseRepositoryTests
{
    private int _testAccountId;
    private int _testClientTypeId;

    [SetUp]
    public void SetUpDependencies()
    {
        var account = new AccountDTO
        {
            Username = "User_".AddGuid(),
            PasswordHash = "HashedPassword123!",
            Email = $"user_{Guid.NewGuid()}@test.com",
            AccountType = 1,
            FirstName = "John",
            LastName = "Doe"
        };
        _testAccountId = UnitOfWork.AccountRepository.Insert(account);

        var clientType = new ClientTypeDTO
        {
            Name = "Type_".AddGuid(),
            Description = "Test Type Description"
        };
        _testClientTypeId = UnitOfWork.ClientTypeRepository.Insert(clientType);
    }

    private ClientDTO CreateValidClient(int? accountId = null, int? clientTypeId = null) => new()
    {
        AccountId = accountId ?? _testAccountId,
        ClientTypeId = clientTypeId ?? _testClientTypeId,
        FirstName = "John",
        LastName = "Doe",
        PhoneNumber = $"+1{Random.Shared.NextInt64(1000000000, 9999999999)}",
        ContactEmail = $"client_{Guid.NewGuid()}@test.com"
    };

    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var client = CreateValidClient();

        // Act
        var newId = UnitOfWork.ClientRepository.Insert(client);
        var insertedClient = UnitOfWork.ClientRepository.GetById(newId);

        // Assert
        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(insertedClient, Is.Not.Null);
        Assert.That(insertedClient!.Id, Is.EqualTo(newId));
        Assert.That(insertedClient.FirstName, Is.EqualTo(client.FirstName));
        Assert.That(insertedClient.LastName, Is.EqualTo(client.LastName));
        Assert.That(insertedClient.PhoneNumber, Is.EqualTo(client.PhoneNumber));
        Assert.That(insertedClient.ContactEmail, Is.EqualTo(client.ContactEmail));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicatePhoneNumber()
    {
        // Arrange
        var first = CreateValidClient();
        var second = CreateValidClient();
        second.PhoneNumber = first.PhoneNumber;

        UnitOfWork.ClientRepository.Insert(first);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.Insert(second));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateContactEmail()
    {
        // Arrange
        var first = CreateValidClient();
        var second = CreateValidClient();
        second.ContactEmail = first.ContactEmail;

        UnitOfWork.ClientRepository.Insert(first);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.Insert(second));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidAccountId()
    {
        // Arrange
        var client = CreateValidClient(accountId: -999);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.Insert(client));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidClientTypeId()
    {
        // Arrange
        var client = CreateValidClient(clientTypeId: -999);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.Insert(client));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyFirstName()
    {
        // Arrange
        var client = CreateValidClient();
        client.FirstName = "   ";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.Insert(client));
    }

    [Test]
    public void Insert_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.ClientRepository.Insert((ClientDTO)null!));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var client = CreateValidClient();
        var id = UnitOfWork.ClientRepository.Insert(client);
        var inserted = UnitOfWork.ClientRepository.GetById(id)!;

        inserted.FirstName = "Jane";
        inserted.LastName = "Smith";

        // Act
        UnitOfWork.ClientRepository.Update(inserted);
        var updated = UnitOfWork.ClientRepository.GetById(id);

        // Assert
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.FirstName, Is.EqualTo("Jane"));
        Assert.That(updated.LastName, Is.EqualTo("Smith"));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateDuplicatePhoneNumber()
    {
        // Arrange
        var first = CreateValidClient();
        var second = CreateValidClient();

        var firstId = UnitOfWork.ClientRepository.Insert(first);
        var secondId = UnitOfWork.ClientRepository.Insert(second);

        var firstClient = UnitOfWork.ClientRepository.GetById(firstId)!;
        var secondClient = UnitOfWork.ClientRepository.GetById(secondId)!;

        secondClient.PhoneNumber = firstClient.PhoneNumber;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.Update(secondClient));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateDuplicateContactEmail()
    {
        // Arrange
        var first = CreateValidClient();
        var second = CreateValidClient();

        var firstId = UnitOfWork.ClientRepository.Insert(first);
        var secondId = UnitOfWork.ClientRepository.Insert(second);

        var firstClient = UnitOfWork.ClientRepository.GetById(firstId)!;
        var secondClient = UnitOfWork.ClientRepository.GetById(secondId)!;

        secondClient.ContactEmail = firstClient.ContactEmail;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.Update(secondClient));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateNonExistentId()
    {
        // Arrange
        var nonExistent = CreateValidClient();
        nonExistent.Id = -999;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.Update(nonExistent));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.ClientRepository.Update((ClientDTO)null!));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var client = CreateValidClient();
        var newId = UnitOfWork.ClientRepository.Insert(client);

        // Act
        UnitOfWork.ClientRepository.Delete(newId);
        var deleted = UnitOfWork.ClientRepository.GetById(newId);

        // Assert
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public void DeleteTest_ShouldNotDeleteInvalidId()
    {
        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.Delete(-999));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.ClientRepository.Delete((object)null!));
    }

    [Test]
    public void GetById_ShouldReturnCorrectClient()
    {
        // Arrange
        var client = CreateValidClient();
        var id = UnitOfWork.ClientRepository.Insert(client);

        // Act
        var result = UnitOfWork.ClientRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }

    [Test]
    public void GetById_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.ClientRepository.GetById(-1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenDeleted_ShouldReturnNull()
    {
        // Arrange
        var client = CreateValidClient();
        var id = UnitOfWork.ClientRepository.Insert(client);
        UnitOfWork.ClientRepository.Delete(id);

        // Act
        var result = UnitOfWork.ClientRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.ClientRepository.GetById((object)null!));
    }

    [Test]
    public void GetByAccountId_ShouldReturnCorrectClient()
    {
        // Arrange
        var client = CreateValidClient();
        UnitOfWork.ClientRepository.Insert(client);

        // Act
        var result = UnitOfWork.ClientRepository.GetByAccountId(client.AccountId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.AccountId, Is.EqualTo(client.AccountId));
    }

    [Test]
    public void GetByPhoneNumber_ShouldReturnCorrectClient()
    {
        // Arrange
        var client = CreateValidClient();
        UnitOfWork.ClientRepository.Insert(client);

        // Act
        var result = UnitOfWork.ClientRepository.GetByPhoneNumber(client.PhoneNumber!);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.PhoneNumber, Is.EqualTo(client.PhoneNumber));
    }

    [Test]
    public void GetByContactEmail_ShouldReturnCorrectClient()
    {
        // Arrange
        var client = CreateValidClient();
        UnitOfWork.ClientRepository.Insert(client);

        // Act
        var result = UnitOfWork.ClientRepository.GetByContactEmail(client.ContactEmail!);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ContactEmail, Is.EqualTo(client.ContactEmail));
    }

    [Test]
    public void GetByClientTypeId_ShouldReturnMatchingClients()
    {
        // Arrange
        var first = CreateValidClient();
        var second = CreateValidClient();

        var id1 = UnitOfWork.ClientRepository.Insert(first);
        var id2 = UnitOfWork.ClientRepository.Insert(second);

        // Act
        var results = UnitOfWork.ClientRepository.GetByClientTypeId(_testClientTypeId).ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == id1), Is.True);
        Assert.That(results.Any(c => c.Id == id2), Is.True);
        Assert.That(results.All(c => c.ClientTypeId == _testClientTypeId), Is.True);
    }

    [Test]
    public void GetAll_ShouldExcludeDeletedClients()
    {
        // Arrange
        var active = CreateValidClient();
        var deleted = CreateValidClient();

        var activeId = UnitOfWork.ClientRepository.Insert(active);
        var deletedId = UnitOfWork.ClientRepository.Insert(deleted);

        UnitOfWork.ClientRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.ClientRepository.GetAll().ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == activeId), Is.True);
        Assert.That(results.Any(c => c.Id == deletedId), Is.False);
    }

    [Test]
    public void GetDeletedClients_ShouldReturnOnlyDeletedClients()
    {
        // Arrange
        var active = CreateValidClient();
        var deleted = CreateValidClient();

        var activeId = UnitOfWork.ClientRepository.Insert(active);
        var deletedId = UnitOfWork.ClientRepository.Insert(deleted);

        UnitOfWork.ClientRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.ClientRepository.GetDeletedClients().ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == deletedId), Is.True);
        Assert.That(results.Any(c => c.Id == activeId), Is.False);
    }
}