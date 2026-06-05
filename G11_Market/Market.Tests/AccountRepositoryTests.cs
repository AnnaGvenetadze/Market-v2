using Market.DTO;
using Market.Repositories;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class AccountRepositoryTests
{
    private const string ConnectionString = "Server=localhost;Database=MarketDB_Test;Trusted_Connection=True;TrustServerCertificate=True;";
    private SqlConnection _connection;
    private AccountRepository _accountRepository;

    [SetUp]
    public void Setup()
    {
        _connection = new SqlConnection(ConnectionString);
        _accountRepository = new AccountRepository(_connection);
    }

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
        var insertedId = _accountRepository.Insert(account);
        var insertedAccount = _accountRepository.GetById(insertedId);

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
        var insertedId = _accountRepository.Insert(account);

        var accountToUpdate = _accountRepository.GetById(insertedId);
        accountToUpdate.PasswordHash = "NewPassword";
        accountToUpdate.LastName = "NotMania";
        accountToUpdate.UpdateDate = DateTime.Now;

        // Act
        _accountRepository.Update(accountToUpdate);
        var updatedAccount = _accountRepository.GetById(insertedId);

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
        var insertedId = _accountRepository.Insert(account);

        // Act
        _accountRepository.Delete(insertedId);

        // Assert
        Assert.Throws<InvalidOperationException>(() => _accountRepository.GetById(insertedId));
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
        _accountRepository.Insert(account1);

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
        Assert.Throws<SqlException>(() => _accountRepository.Insert(account2));
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
        _accountRepository.Insert(account1);

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
        Assert.Throws<SqlException>(() => _accountRepository.Insert(account2));
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
        _accountRepository.Insert(account);

        // Act
        var result = _accountRepository.GetByUsername(wantedUsername);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Username, Is.EqualTo(wantedUsername));
    }

    [Test]
    public void GetByUsername_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = _accountRepository.GetByUsername("NonExistentUser");

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
        _accountRepository.Insert(account);

        // Act
        var result = _accountRepository.GetByEmail(targetEmail);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(targetEmail));
    }

    [Test]
    public void GetByEmail_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = _accountRepository.GetByEmail("notfound@gmail.com");

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
        var account3 = new AccountDTO { Username = "U3".AddGuid(), PasswordHash = "H", Email = "E3".AddGuid() + "@gmail.com", AccountType = 99, FirstName = "E", LastName = "F" }; 

        var id1 = _accountRepository.Insert(account1);
        var id2 = _accountRepository.Insert(account2);
        _accountRepository.Insert(account3);

        // Act
        var results = _accountRepository.GetByAccountType(targetType).ToList();

        // Assert
        Assert.That(results.Count, Is.AtLeast(2));
        Assert.That(results.Any(x => x.Id == id1), Is.True);
        Assert.That(results.Any(x => x.Id == id2), Is.True);
        Assert.That(results.All(x => x.AccountType == targetType), Is.True);
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => _accountRepository.GetById(null!));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => _accountRepository.Update(null!));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => _accountRepository.Delete(null!));
    }

    [Test]
    public void GetAll_ShouldReturnAllInsertedRecords()
    {
        // Arrange
        var account1 = new AccountDTO { Username = "All1".AddGuid(), PasswordHash = "H", Email = "All1".AddGuid() + "@m.com", AccountType = 1, FirstName = "A", LastName = "B" };
        var account2 = new AccountDTO { Username = "All2".AddGuid(), PasswordHash = "H", Email = "All2".AddGuid() + "@m.com", AccountType = 1, FirstName = "C", LastName = "D" };

        _accountRepository.Insert(account1);
        _accountRepository.Insert(account2);

        // Act
        var allAccounts = _accountRepository.GetAll().ToList();

        // Assert
        Assert.That(allAccounts.Count, Is.AtLeast(2));
    }

    [TearDown]
    public void TearDown()
    {
        _accountRepository.Dispose();
        _connection.Dispose();
    }
}
