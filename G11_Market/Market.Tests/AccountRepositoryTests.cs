using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class AccountRepositoryTests : BaseRepositoryTests
{
    private static AccountDTO CreateValidAccount() => new()
    {
        Username = "User_".AddGuid(),
        PasswordHash = "HashedPassword123!",
        Email = $"user_{Guid.NewGuid()}@test.com",
        AccountType = 1, // 1: Employee, 2: Individual Client, 3: Corporate Client
        FirstName = "John",
        LastName = "Doe"
    };

    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var account = CreateValidAccount();

        // Act
        var newId = UnitOfWork.AccountRepository.AssignAttribute(account);
        var insertedAccount = UnitOfWork.AccountRepository.GetById(newId);

        // Assert
        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(insertedAccount, Is.Not.Null);
        Assert.That(insertedAccount!.Id, Is.EqualTo(newId));
        Assert.That(insertedAccount.Username, Is.EqualTo(account.Username));
        Assert.That(insertedAccount.Email, Is.EqualTo(account.Email));
        Assert.That(insertedAccount.AccountType, Is.EqualTo(account.AccountType));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateUsername()
    {
        // Arrange
        var first = CreateValidAccount();
        var second = CreateValidAccount();
        second.Username = first.Username;

        UnitOfWork.AccountRepository.AssignAttribute(first);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.AssignAttribute(second));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateEmail()
    {
        // Arrange
        var first = CreateValidAccount();
        var second = CreateValidAccount();
        second.Email = first.Email;

        UnitOfWork.AccountRepository.AssignAttribute(first);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.AssignAttribute(second));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidAccountType()
    {
        // Arrange
        var account = CreateValidAccount();
        account.AccountType = 99;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.AssignAttribute(account));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyUsername()
    {
        // Arrange
        var account = CreateValidAccount();
        account.Username = "   ";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.AssignAttribute(account));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyEmail()
    {
        // Arrange
        var account = CreateValidAccount();
        account.Email = "";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.AssignAttribute(account));
    }

    [Test]
    public void Insert_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.AccountRepository.AssignAttribute(null!));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var account = CreateValidAccount();
        var newId = UnitOfWork.AccountRepository.AssignAttribute(account);
        var insertedAccount = UnitOfWork.AccountRepository.GetById(newId)!;

        insertedAccount.FirstName = "Jane";
        insertedAccount.LastName = "Smith";
        insertedAccount.AccountType = 2;

        // Act
        UnitOfWork.AccountRepository.Update(insertedAccount);
        var updatedAccount = UnitOfWork.AccountRepository.GetById(newId);

        // Assert
        Assert.That(updatedAccount, Is.Not.Null);
        Assert.That(updatedAccount!.FirstName, Is.EqualTo("Jane"));
        Assert.That(updatedAccount.LastName, Is.EqualTo("Smith"));
        Assert.That(updatedAccount.AccountType, Is.EqualTo(2));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateDuplicateUsername()
    {
        // Arrange
        var first = CreateValidAccount();
        var second = CreateValidAccount();

        var firstId = UnitOfWork.AccountRepository.AssignAttribute(first);
        var secondId = UnitOfWork.AccountRepository.AssignAttribute(second);

        var firstAccount = UnitOfWork.AccountRepository.GetById(firstId)!;
        var secondAccount = UnitOfWork.AccountRepository.GetById(secondId)!;

        secondAccount.Username = firstAccount.Username;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.Update(secondAccount));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateDuplicateEmail()
    {
        // Arrange
        var first = CreateValidAccount();
        var second = CreateValidAccount();

        var firstId = UnitOfWork.AccountRepository.AssignAttribute(first);
        var secondId = UnitOfWork.AccountRepository.AssignAttribute(second);

        var firstAccount = UnitOfWork.AccountRepository.GetById(firstId)!;
        var secondAccount = UnitOfWork.AccountRepository.GetById(secondId)!;

        secondAccount.Email = firstAccount.Email;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.Update(secondAccount));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateInvalidAccountType()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.AssignAttribute(account);
        var inserted = UnitOfWork.AccountRepository.GetById(id)!;

        inserted.AccountType = 0;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.Update(inserted));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateEmptyUsername()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.AssignAttribute(account);
        var inserted = UnitOfWork.AccountRepository.GetById(id)!;

        inserted.Username = "   ";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.Update(inserted));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateNonExistentId()
    {
        // Arrange
        var nonExistent = CreateValidAccount();
        nonExistent.Id = -999;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.Update(nonExistent));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.AccountRepository.Update(null!));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData() 
    {
        // Arrange
        var account = CreateValidAccount();
        var newId = UnitOfWork.AccountRepository.AssignAttribute(account);

        // Act
        UnitOfWork.AccountRepository.Delete(newId);
        var deletedAccount = UnitOfWork.AccountRepository.GetById(newId);

        // Assert
        Assert.That(deletedAccount, Is.Null);
    }

    [Test]
    public void DeleteTest_ShouldNotDeleteInvalidId()
    {
        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.Delete(-999));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.AccountRepository.Delete(null!));
    }

    [Test]
    public void GetById_ShouldReturnCorrectAccount()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.AssignAttribute(account);

        // Act
        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }

    [Test]
    public void GetById_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.AccountRepository.GetById(-1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenDeleted_ShouldReturnNull()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.AssignAttribute(account);
        UnitOfWork.AccountRepository.Delete(id);

        // Act
        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.AccountRepository.GetById((object)null!));
    }

    [Test]
    public void GetByUsername_ShouldReturnCorrectAccount()
    {
        // Arrange
        var account = CreateValidAccount();
        UnitOfWork.AccountRepository.AssignAttribute(account);

        // Act
        var result = UnitOfWork.AccountRepository.GetByUsername(account.Username);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo(account.Username));
    }

    [Test]
    public void GetByUsername_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.AccountRepository.GetByUsername("NonExistentUser_".AddGuid());

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetByEmail_ShouldReturnCorrectAccount()
    {
        // Arrange
        var account = CreateValidAccount();
        UnitOfWork.AccountRepository.AssignAttribute(account);

        // Act
        var result = UnitOfWork.AccountRepository.GetByEmail(account.Email);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Email, Is.EqualTo(account.Email));
    }

    [Test]
    public void GetByEmail_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.AccountRepository.GetByEmail($"nonexistent_{Guid.NewGuid()}@test.com");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetByType_ShouldReturnMatchingAccounts()
    {
        // Arrange
        byte targetType = 3;
        var first = CreateValidAccount();
        first.AccountType = targetType;
        var second = CreateValidAccount();
        second.AccountType = targetType;

        var id1 = UnitOfWork.AccountRepository.AssignAttribute(first);
        var id2 = UnitOfWork.AccountRepository.AssignAttribute(second);

        // Act
        var results = UnitOfWork.AccountRepository.GetByAccountType(targetType).ToList();

        // Assert
        Assert.That(results.Any(a => a.Id == id1), Is.True);
        Assert.That(results.Any(a => a.Id == id2), Is.True);
        Assert.That(results.All(a => a.AccountType == targetType), Is.True);
    }

    [Test]
    public void GetAll_ShouldExcludeDeletedAccounts()
    {
        // Arrange
        var active = CreateValidAccount();
        var deleted = CreateValidAccount();

        var activeId = UnitOfWork.AccountRepository.AssignAttribute(active);
        var deletedId = UnitOfWork.AccountRepository.AssignAttribute(deleted);

        UnitOfWork.AccountRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.AccountRepository.GetAll().ToList();

        // Assert
        Assert.That(results.Any(a => a.Id == activeId), Is.True);
        Assert.That(results.Any(a => a.Id == deletedId), Is.False);
    }
}