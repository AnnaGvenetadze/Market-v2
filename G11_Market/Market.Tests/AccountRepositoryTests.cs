using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class AccountRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var account = new AccountDTO
        {
            Username = "User123".AddGuid(),
            PasswordHash = "HashedPassword123",
            Email = "Email123".AddGuid() + "@gmail.com",
            AccountType = 1, 
            FirstName = "Luka",
            LastName = "Mania",
            IsDeleted = false,
            CreateDate = DateTime.Now
        };

        // Act
        var insertedId = UnitOfWork.AccountRepository.Insert(account);
        var insertedAccount = UnitOfWork.AccountRepository.GetById(insertedId);

        // Assert
        Assert.That(insertedAccount, Is.Not.Null);
        Assert.That(insertedAccount.Id, Is.EqualTo(insertedId));
        Assert.That(insertedAccount.Username, Is.EqualTo(account.Username));
        Assert.That(insertedAccount.Email, Is.EqualTo(account.Email));
        Assert.That(insertedAccount.AccountType, Is.EqualTo(1));
    }


    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var account = new AccountDTO
        {
            Username = "User123".AddGuid(),
            PasswordHash = "OldPass",
            Email = "Email123".AddGuid() + "@gmail.com",
            AccountType = 1,
            FirstName = "Luka",
            LastName = "Mania"
        };
        var insertedId = UnitOfWork.AccountRepository.Insert(account);

        var accountToUpdate = UnitOfWork.AccountRepository.GetById(insertedId);
        accountToUpdate!.PasswordHash = "NewPassword";
        accountToUpdate.LastName = "NotMania";
        accountToUpdate.UpdateDate = DateTime.Now;

        // Act
        UnitOfWork.AccountRepository.Update(accountToUpdate);
        var updatedAccount = UnitOfWork.AccountRepository.GetById(insertedId);

        // Assert
        Assert.That(updatedAccount, Is.Not.Null);
        Assert.That(updatedAccount.PasswordHash, Is.EqualTo("NewPassword"));
        Assert.That(updatedAccount.LastName, Is.EqualTo("NotMania"));
    }


    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var account = new AccountDTO
        {
            Username = "User123".AddGuid(),
            PasswordHash = "OldPass",
            Email = "Email123".AddGuid() + "@gmail.com",
            AccountType = 1,
            FirstName = "Luka",
            LastName = "Mania"
        };
        var insertedId = UnitOfWork.AccountRepository.Insert(account);

        // Act
        UnitOfWork.AccountRepository.Delete(insertedId);

        // Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.GetById(insertedId));
    }


    [Test]
    public void InsertTest_ShouldNotInsertDuplicateUsername()
    {
        // Arrange
        var identicalUsername = "User123".AddGuid();
        var account1 = new AccountDTO
        {
            Username = identicalUsername,
            PasswordHash = "Pass11",
            Email = "Email1".AddGuid() + "@gmail.com",
            AccountType = 1,
            FirstName = "User",
            LastName = "One"
        };
        UnitOfWork.AccountRepository.Insert(account1);

        var account2 = new AccountDTO
        {
            Username = identicalUsername,
            PasswordHash = "Hash2",
            Email = "Email2".AddGuid() + "@gmail.com",
            AccountType = 1,
            FirstName = "User",
            LastName = "Two"
        };

        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.Insert(account2));
    }


    [Test]
    public void InsertTest_ShouldNotInsertDuplicateEmail()
    {
        // Arrange
        var sharedEmail = "Email123".AddGuid() + "@gmail.com";
        var account1 = new AccountDTO
        {
            Username = "User1".AddGuid(),
            PasswordHash = "Hash1",
            Email = sharedEmail,
            AccountType = 1,
            FirstName = "User",
            LastName = "One"
        };
        UnitOfWork.AccountRepository.Insert(account1);

        var account2 = new AccountDTO
        {
            Username = "User2".AddGuid(),
            PasswordHash = "Hash2",
            Email = sharedEmail,
            AccountType = 1,
            FirstName = "User",
            LastName = "Two"
        };

        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AccountRepository.Insert(account2));
    }


    [Test]
    public void GetByUsername_ShouldReturnCorrectAccount()
    {
        // Arrange
        var wantedUsername = "User".AddGuid();
        var account = new AccountDTO
        {
            Username = wantedUsername,
            PasswordHash = "Password",
            Email = "Email".AddGuid() + "@gmail.com",
            AccountType = 1,
            FirstName = "Luka",
            LastName = "Mania"
        };
        UnitOfWork.AccountRepository.Insert(account);

        // Act
        var result = UnitOfWork.AccountRepository.GetByUsername(wantedUsername);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Username, Is.EqualTo(wantedUsername));
    }


    [Test]
    public void GetByUsername_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.AccountRepository.GetByUsername("NonExistentUser");

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetByEmail_ShouldReturnCorrectAccount()
    {
        // Arrange
        var targetEmail = "TargetEmail".AddGuid() + "@gmail.com";
        var account = new AccountDTO
        {
            Username = "User".AddGuid(),
            PasswordHash = "Pass1",
            Email = targetEmail,
            AccountType = 1,
            FirstName = "Target",
            LastName = "Email"
        };
        UnitOfWork.AccountRepository.Insert(account);

        // Act
        var result = UnitOfWork.AccountRepository.GetByEmail(targetEmail);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(targetEmail));
    }


    [Test]
    public void GetByEmail_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.AccountRepository.GetByEmail("notfound@gmail.com");

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetByAccountType_ShouldReturnMatchingAccounts()
    {
        // Arrange
        byte targetType = 2;
        var account1 = new AccountDTO { Username = "U1".AddGuid(), PasswordHash = "H", Email = "E1".AddGuid() + "@gmail.com", AccountType = targetType, FirstName = "A", LastName = "B" };
        var account2 = new AccountDTO { Username = "U2".AddGuid(), PasswordHash = "H", Email = "E2".AddGuid() + "@gmail.com", AccountType = targetType, FirstName = "C", LastName = "D" };
        var account3 = new AccountDTO { Username = "U3".AddGuid(), PasswordHash = "H", Email = "E3".AddGuid() + "@gmail.com", AccountType = 3, FirstName = "E", LastName = "F" }; 

        var id1 = UnitOfWork.AccountRepository.Insert(account1);
        var id2 = UnitOfWork.AccountRepository.Insert(account2);
        UnitOfWork.AccountRepository.Insert(account3); 

        // Act
        var results = UnitOfWork.AccountRepository.GetByAccountType(targetType).ToList();

        // Assert
        Assert.That(results.Count, Is.AtLeast(2));
        Assert.That(results.Any(x => x.Id == id1), Is.True);
        Assert.That(results.Any(x => x.Id == id2), Is.True);
        Assert.That(results.All(x => x.AccountType == targetType), Is.True);
    }


    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.AccountRepository.GetById(null!));
    }


    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.AccountRepository.Update(null!));
    }


    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.AccountRepository.Delete(null!));
    }


    [Test]
    public void GetAll_ShouldReturnAllInsertedRecords()
    {
        // Arrange
        var account1 = new AccountDTO { Username = "All1".AddGuid(), PasswordHash = "H", Email = "All1".AddGuid() + "@m.com", AccountType = 1, FirstName = "A", LastName = "B" };
        var account2 = new AccountDTO { Username = "All2".AddGuid(), PasswordHash = "H", Email = "All2".AddGuid() + "@m.com", AccountType = 1, FirstName = "C", LastName = "D" };

        UnitOfWork.AccountRepository.Insert(account1);
        UnitOfWork.AccountRepository.Insert(account2);

        // Act
        var allAccounts = UnitOfWork.AccountRepository.GetAll().ToList();

        // Assert
        Assert.That(allAccounts.Count, Is.AtLeast(2));
    }
}
