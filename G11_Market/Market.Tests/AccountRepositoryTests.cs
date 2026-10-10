
using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System.Security.Cryptography;
using System.Text;

namespace Market.Tests;

[TestFixture]
public class AccountRepositoryTests : BaseRepositoryTests
{
    private static AccountDTO CreateValidAccount() => new()
    {
        Username = "User".AddGuid(),
        PasswordHash = "HashedPassword123!",
        IsActive = true
    };


    [Test]
    public void Insert_ShouldInsertValidAccount()
    {
        // Arrange
        var account = CreateValidAccount();

        // Act
        var id = UnitOfWork.AccountRepository.Insert(account);
        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(id, Is.GreaterThan(0));
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id));
            Assert.That(result.Username, Is.EqualTo(account.Username));
            Assert.That(result.PasswordHash, Is.EqualTo(account.PasswordHash));
            Assert.That(result.IsActive, Is.True);
            Assert.That(result.IsDeleted, Is.False);
            Assert.That(result.FailedLoginAttempts, Is.Zero);
            Assert.That(result.CreateDate, Is.Not.EqualTo(default(DateTime)));
            Assert.That(result.Token, Is.Null);
            Assert.That(result.TokenExpiration, Is.Null);
        });
    }


    [Test]
    public void Insert_WhenUsernameAlreadyExists_ShouldThrowSqlException()
    {
        // Arrange
        var first = CreateValidAccount();
        var second = CreateValidAccount();
        second.Username = first.Username;

        UnitOfWork.AccountRepository.Insert(first);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.Insert(second));
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Insert_WhenUsernameIsInvalid_ShouldThrowSqlException(string? username)
    {
        // Arrange
        var account = CreateValidAccount();
        account.Username = username!;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.Insert(account));
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Insert_WhenPasswordHashIsInvalid_ShouldThrowSqlException(string? passwordHash)
    {
        // Arrange
        var account = CreateValidAccount();
        account.PasswordHash = passwordHash!;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.Insert(account));
    }


    [Test]
    public void Insert_WhenAccountIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.AccountRepository.Insert(null!));
    }


    [Test]
    public void Insert_WhenAccountIsInactive_ShouldSaveInactiveAccount()
    {
        // Arrange
        var account = CreateValidAccount();
        account.IsActive = false;

        // Act
        var id = UnitOfWork.AccountRepository.Insert(account);
        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsActive, Is.False);
            Assert.That(result.IsDeleted, Is.False);
        });
    }


    [Test]
    public void Update_ShouldUpdateAccount()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);
        var inserted = UnitOfWork.AccountRepository.GetById(id)!;

        inserted.Username = "Updated".AddGuid();
        inserted.PasswordHash = "UpdatedPasswordHash123!";
        inserted.IsActive = false;

        // Act
        UnitOfWork.AccountRepository.Update(inserted);
        var updated = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.Username, Is.EqualTo(inserted.Username));
            Assert.That(updated.PasswordHash, Is.EqualTo(inserted.PasswordHash));
            Assert.That(updated.IsActive, Is.False);
            Assert.That(updated.UpdateDate, Is.Not.Null);
        });
    }


    [Test]
    public void Update_WhenUsernameAlreadyExists_ShouldThrowSqlException()
    {
        // Arrange
        var first = CreateValidAccount();
        var second = CreateValidAccount();

        var firstId = UnitOfWork.AccountRepository.Insert(first);
        var secondId = UnitOfWork.AccountRepository.Insert(second);

        var firstAccount = UnitOfWork.AccountRepository.GetById(firstId)!;
        var secondAccount = UnitOfWork.AccountRepository.GetById(secondId)!;

        secondAccount.Username = firstAccount.Username;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.Update(secondAccount));
    }


    [Test]
    public void Update_WhenUsernameIsEmpty_ShouldThrowSqlException()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);
        var inserted = UnitOfWork.AccountRepository.GetById(id)!;

        inserted.Username = "   ";

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.Update(inserted));
    }


    [Test]
    public void Update_WhenAccountDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var account = CreateValidAccount();
        account.Id = int.MaxValue;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.Update(account));
    }


    [Test]
    public void Update_WhenAccountIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.AccountRepository.Update(null!));
    }


    [Test]
    public void Delete_ShouldSoftDeleteAccount()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act
        UnitOfWork.AccountRepository.Delete(id);
        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void Delete_WhenAccountDoesNotExist_ShouldThrowSqlException()
    {
        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.Delete(int.MaxValue));
    }


    [Test]
    public void Delete_WhenAlreadyDeleted_ShouldThrowSqlException()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        UnitOfWork.AccountRepository.Delete(id);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.Delete(id));
    }


    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.AccountRepository.Delete(null!));
    }


    [Test]
    public void Restore_ShouldRestoreDeletedAccount()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        UnitOfWork.AccountRepository.Delete(id);

        // Act
        UnitOfWork.AccountRepository.Restore(id);
        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id));
            Assert.That(result.IsDeleted, Is.False);
        });
    }


    [Test]
    public void Restore_WhenAccountDoesNotExist_ShouldThrowSqlException()
    {
        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.Restore(int.MaxValue));
    }


    [Test]
    public void Restore_WhenAccountIsNotDeleted_ShouldThrowSqlException()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.Restore(id));
    }


    [Test]
    public void GetById_ShouldReturnCorrectAccount()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act
        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id));
            Assert.That(result.Username, Is.EqualTo(account.Username));
        });
    }


    [Test]
    public void GetById_WhenAccountDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.AccountRepository.GetById(int.MaxValue);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetById_WhenAccountIsDeleted_ShouldReturnNull()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        UnitOfWork.AccountRepository.Delete(id);

        // Act
        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.AccountRepository.GetById(null!));
    }


    [Test]
    public void GetAll_ShouldExcludeDeletedAccounts()
    {
        // Arrange
        var first = CreateValidAccount();
        var second = CreateValidAccount();

        var firstId = UnitOfWork.AccountRepository.Insert(first);
        var secondId = UnitOfWork.AccountRepository.Insert(second);

        UnitOfWork.AccountRepository.Delete(secondId);

        // Act
        var results = UnitOfWork.AccountRepository.GetAll().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(a => a.Id == firstId), Is.True);
            Assert.That(results.Any(a => a.Id == secondId), Is.False);
        });
    }


    [Test]
    public void GetAll_ShouldIncludeInactiveAccounts()
    {
        // Arrange
        var account = CreateValidAccount();
        account.IsActive = false;

        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act
        var results = UnitOfWork.AccountRepository.GetAll().ToList();

        // Assert
        Assert.That(
            results.Any(a => a.Id == id && !a.IsActive),
            Is.True);
    }


    [Test]
    public void GetByUsername_ShouldReturnCorrectAccount()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act
        var result = UnitOfWork.AccountRepository.GetByUsername(account.Username);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id));
            Assert.That(result.Username, Is.EqualTo(account.Username));
        });
    }


    [Test]
    public void GetByUsername_WithWhitespace_ShouldTrimUsername()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act
        var result = UnitOfWork.AccountRepository
            .GetByUsername($"  {account.Username}  ");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }


    [Test]
    public void GetByUsername_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.AccountRepository
            .GetByUsername("Missing".AddGuid());

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetByUsername_WhenAccountIsDeleted_ShouldReturnNull()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        UnitOfWork.AccountRepository.Delete(id);

        // Act
        var result = UnitOfWork.AccountRepository
            .GetByUsername(account.Username);

        // Assert
        Assert.That(result, Is.Null);
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void GetByUsername_WhenUsernameIsInvalid_ShouldThrowArgumentException(
        string? username)
    {
        // Act & Assert
        Assert.Catch<ArgumentException>(() =>
            UnitOfWork.AccountRepository.GetByUsername(username!));
    }


    [Test]
    public async Task GetByUsernameAsync_ShouldReturnCorrectAccount()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act
        var result = await UnitOfWork.AccountRepository
            .GetByUsernameAsync(account.Username);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }


    [Test]
    public async Task GetByUsernameAsync_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = await UnitOfWork.AccountRepository
            .GetByUsernameAsync("Missing".AddGuid());

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void UpdateLoginAttempts_ShouldUpdateLoginInformation()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        var lastLogin = DateTime.UtcNow;
        var lockoutEnd = DateTime.UtcNow.AddMinutes(15);

        // Act
        UnitOfWork.AccountRepository.UpdateLoginAttempts(
            id,
            5,
            lastLogin,
            lockoutEnd);

        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FailedLoginAttempts, Is.EqualTo(5));
            Assert.That(result.LastLoginAtUtc,
                Is.EqualTo(lastLogin).Within(TimeSpan.FromSeconds(1)));
            Assert.That(result.LockoutEndUtc,
                Is.EqualTo(lockoutEnd).Within(TimeSpan.FromSeconds(1)));
        });
    }


    [TestCase(0)]
    [TestCase(-1)]
    public void UpdateLoginAttempts_WhenIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int accountId)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.AccountRepository.UpdateLoginAttempts(
                accountId, 1, null, null));
    }


    [Test]
    public void UpdateLoginAttempts_WhenFailedAttemptsIsNegative_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.AccountRepository.UpdateLoginAttempts(
                id, -1, null, null));
    }


    [Test]
    public void UpdateLoginAttempts_WhenAccountDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            UnitOfWork.AccountRepository.UpdateLoginAttempts(
                int.MaxValue, 1, null, null));
    }


    [Test]
    public async Task RecordFailedLoginAsync_ShouldIncreaseFailedAttempts()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act
        await UnitOfWork.AccountRepository.RecordFailedLoginAsync(
            id,
            DateTime.UtcNow,
            5,
            TimeSpan.FromMinutes(15));

        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FailedLoginAttempts, Is.EqualTo(1));
            Assert.That(result.LockoutEndUtc, Is.Null);
        });
    }


    [Test]
    public async Task RecordFailedLoginAsync_WhenMaxAttemptsReached_ShouldSetLockoutTime()
    {
        // Arrange
        var account = CreateValidAccount();
        account.FailedLoginAttempts = 4;

        var id = UnitOfWork.AccountRepository.Insert(account);
        var failedAttemptTime = DateTime.UtcNow;

        // Act
        await UnitOfWork.AccountRepository.RecordFailedLoginAsync(
            id,
            failedAttemptTime,
            5,
            TimeSpan.FromMinutes(15));

        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FailedLoginAttempts, Is.EqualTo(5));
            Assert.That(result.LockoutEndUtc, Is.Not.Null);
            Assert.That(result.LockoutEndUtc,
                Is.EqualTo(failedAttemptTime.AddMinutes(15))
                    .Within(TimeSpan.FromSeconds(1)));
        });
    }


    [Test]
    public async Task RecordSuccessfulLoginAsync_ShouldResetAttemptsAndUpdateLastLogin()
    {
        // Arrange
        var account = CreateValidAccount();
        account.FailedLoginAttempts = 3;

        var id = UnitOfWork.AccountRepository.Insert(account);
        var loginTime = DateTime.UtcNow;

        // Act
        await UnitOfWork.AccountRepository.RecordSuccessfulLoginAsync(
            id,
            loginTime,
            null);

        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FailedLoginAttempts, Is.Zero);
            Assert.That(result.LastLoginAtUtc,
                Is.EqualTo(loginTime).Within(TimeSpan.FromSeconds(1)));
            Assert.That(result.PasswordHash, Is.EqualTo(account.PasswordHash));
        });
    }


    [Test]
    public async Task RecordSuccessfulLoginAsync_WithNewPasswordHash_ShouldUpdatePasswordHash()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);
        var newPasswordHash = "NewHashedPassword123!";

        // Act
        await UnitOfWork.AccountRepository.RecordSuccessfulLoginAsync(
            id,
            DateTime.UtcNow,
            newPasswordHash);

        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.PasswordHash, Is.EqualTo(newPasswordHash));
    }


    [Test]
    public async Task RefreshTokenAsync_ShouldSaveTokenHashAndExpiration()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        var tokenHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

        var expiration = DateTime.UtcNow.AddDays(30);

        // Act
        await UnitOfWork.AccountRepository.RefreshTokenAsync(
            id,
            tokenHash,
            expiration);

        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Token, Is.EqualTo(tokenHash));
            Assert.That(result.TokenExpiration,
                Is.EqualTo(expiration).Within(TimeSpan.FromSeconds(1)));
        });
    }


    [TestCase(0)]
    [TestCase(-1)]
    public void RefreshTokenAsync_WhenAccountIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int accountId)
    {
        // Arrange
        var tokenHash = SHA256.HashData(
            Encoding.UTF8.GetBytes("TestToken"));

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await UnitOfWork.AccountRepository.RefreshTokenAsync(
                accountId,
                tokenHash,
                DateTime.UtcNow.AddDays(30)));
    }


    [Test]
    public void RefreshTokenAsync_WhenTokenHashIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await UnitOfWork.AccountRepository.RefreshTokenAsync(
                id,
                null!,
                DateTime.UtcNow.AddDays(30)));
    }


    [Test]
    public void RefreshTokenAsync_WhenTokenHashHasInvalidLength_ShouldThrowArgumentException()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        var invalidHash = new byte[16];

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await UnitOfWork.AccountRepository.RefreshTokenAsync(
                id,
                invalidHash,
                DateTime.UtcNow.AddDays(30)));
    }


    [Test]
    public void RefreshTokenAsync_WhenAccountIsInactive_ShouldThrowSqlException()
    {
        // Arrange
        var account = CreateValidAccount();
        account.IsActive = false;

        var id = UnitOfWork.AccountRepository.Insert(account);

        var tokenHash = SHA256.HashData(
            Encoding.UTF8.GetBytes("TestToken"));

        // Act & Assert
        Assert.ThrowsAsync<SqlException>(async () =>
            await UnitOfWork.AccountRepository.RefreshTokenAsync(
                id,
                tokenHash,
                DateTime.UtcNow.AddDays(30)));
    }


    [Test]
    public async Task GetRolesByAccountIdAsync_ShouldReturnAssignedRoles()
    {
        // Arrange
        var account = CreateValidAccount();
        var accountId = UnitOfWork.AccountRepository.Insert(account);

        var role = UnitOfWork.RoleRepository.GetByName("Administrator");
        Assert.That(role, Is.Not.Null);

        var employee = new EmployeeDTO
        {
            AccountId = accountId,
            FirstName = "John",
            LastName = "Doe",
            EmployeeCode = "EMP".AddGuid(),
            Email = $"u_{Guid.NewGuid():N}@test.com",
            PhoneNumber = "555123456",
            HireDate = DateTime.UtcNow.Date
        };

        var employeeId = UnitOfWork.EmployeeRepository.Insert(employee);

        UnitOfWork.EmployeeRepository.AssignRole(new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = role!.Id
        });

        // Act
        var result = await UnitOfWork.AccountRepository
            .GetRolesByAccountIdAsync(accountId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Any(r => r.Id == role.Id), Is.True);
        });
    }


    [Test]
    public async Task GetRolesByAccountIdAsync_WhenAccountHasNoRoles_ShouldReturnEmpty()
    {
        // Arrange
        var account = CreateValidAccount();
        var accountId = UnitOfWork.AccountRepository.Insert(account);

        // Act
        var result = await UnitOfWork.AccountRepository
            .GetRolesByAccountIdAsync(accountId);

        // Assert
        Assert.That(result, Is.Empty);
    }


    [TestCase(0)]
    [TestCase(-1)]
    public void GetRolesByAccountIdAsync_WhenAccountIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int accountId)
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await UnitOfWork.AccountRepository.GetRolesByAccountIdAsync(accountId));
    }


    [Test]
    public void ResetPassword_ShouldUpdatePasswordHash()
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        var newPasswordHash = "NewHashedPassword123!";

        // Act
        UnitOfWork.AccountRepository.ResetPassword(
            id,
            newPasswordHash);

        var result = UnitOfWork.AccountRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.PasswordHash, Is.EqualTo(newPasswordHash));
    }


    [TestCase(0)]
    [TestCase(-1)]
    public void ResetPassword_WhenAccountIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int accountId)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.AccountRepository.ResetPassword(
                accountId,
                "NewHashedPassword123!"));
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ResetPassword_WhenPasswordHashIsInvalid_ShouldThrowArgumentException(
        string? passwordHash)
    {
        // Arrange
        var account = CreateValidAccount();
        var id = UnitOfWork.AccountRepository.Insert(account);

        // Act & Assert
        Assert.Catch<ArgumentException>(() =>
            UnitOfWork.AccountRepository.ResetPassword(
                id,
                passwordHash!));
    }


    [Test]
    public void ResetPassword_WhenAccountDoesNotExist_ShouldThrowSqlException()
    {
        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.AccountRepository.ResetPassword(
                int.MaxValue,
                "NewHashedPassword123!"));
    }

    [Test]
    public async Task GetRolesByRefreshTokenAsync_ShouldReturnAssignedRoles()
    {
        // Arrange
        var account = CreateValidAccount();
        var accountId = UnitOfWork.AccountRepository.Insert(account);

        var role = UnitOfWork.RoleRepository.GetByName("Administrator");
        Assert.That(role, Is.Not.Null);

        var employee = new EmployeeDTO
        {
            AccountId = accountId,
            FirstName = "Jane",
            LastName = "Doe",
            EmployeeCode = "EMP".AddGuid(),
            Email = $"u_{Guid.NewGuid():N}@test.com",
            PhoneNumber = "555987654",
            HireDate = DateTime.UtcNow.Date
        };

        var employeeId = UnitOfWork.EmployeeRepository.Insert(employee);

        UnitOfWork.EmployeeRepository.AssignRole(new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = role!.Id
        });

        var refreshToken = new RefreshTokenDTO
        {
            AccountId = accountId
        };

        // Act
        var result = await UnitOfWork.AccountRepository
            .GetRolesByRefreshTokenAsync(refreshToken);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Any(r => r.Id == role.Id), Is.True);
        });
    }

    [Test]
    public void GetRolesByRefreshTokenAsync_WhenRefreshTokenIsNull_ShouldThrowArgumentNullException()
    {
        // Act And Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await UnitOfWork.AccountRepository.GetRolesByRefreshTokenAsync(null!));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetRolesByRefreshTokenAsync_WhenAccountIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int accountId)
    {
        // Arrange
        var refreshToken = new RefreshTokenDTO
        {
            AccountId = accountId
        };

        // Act And Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await UnitOfWork.AccountRepository.GetRolesByRefreshTokenAsync(refreshToken));
    }

    [Test]
    public void GetRolesByRefreshTokenAsync_WhenAccountDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var refreshToken = new RefreshTokenDTO
        {
            AccountId = int.MaxValue
        };

        // Act And Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await UnitOfWork.AccountRepository.GetRolesByRefreshTokenAsync(refreshToken));
    }

    [Test]
    public async Task HasRoleAsync_WhenAccountHasRole_ShouldReturnTrue()
    {
        // Arrange
        var account = CreateValidAccount();
        var accountId = UnitOfWork.AccountRepository.Insert(account);

        var role = UnitOfWork.RoleRepository.GetByName("Administrator");
        Assert.That(role, Is.Not.Null);

        var employee = new EmployeeDTO
        {
            AccountId = accountId,
            FirstName = "John",
            LastName = "Doe",
            EmployeeCode = "EMP".AddGuid(),
            Email = $"u_{Guid.NewGuid():N}@test.com",
            PhoneNumber = "555123456",
            HireDate = DateTime.UtcNow.Date
        };

        var employeeId = UnitOfWork.EmployeeRepository.Insert(employee);

        UnitOfWork.EmployeeRepository.AssignRole(new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = role!.Id
        });

        // Act - testing case-insensitivity as well ("administrator" vs "Administrator")
        var result = await UnitOfWork.AccountRepository.HasRoleAsync(accountId, "administrator");

        // Assert
        Assert.That(result, Is.True);
    }


    [Test]
    public async Task HasRoleAsync_WhenAccountDoesNotHaveRole_ShouldReturnFalse()
    {
        // Arrange
        var account = CreateValidAccount();
        var accountId = UnitOfWork.AccountRepository.Insert(account);

        // Act
        var result = await UnitOfWork.AccountRepository.HasRoleAsync(accountId, "Administrator");

        // Assert
        Assert.That(result, Is.False);
    }


    [TestCase(0)]
    [TestCase(-1)]
    public void HasRoleAsync_WhenAccountIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int accountId)
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await UnitOfWork.AccountRepository.HasRoleAsync(accountId, "Administrator"));
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public async Task HasRoleAsync_WhenRoleNameIsInvalid_ShouldThrowArgumentException(
        string? roleName)
    {
        // Arrange
        var account = CreateValidAccount();
        var accountId = UnitOfWork.AccountRepository.Insert(account);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await UnitOfWork.AccountRepository.HasRoleAsync(accountId, roleName!));
    }
}
