//using Dapper;
//using Market.DTO;
//using Market.Extensions;
//using Market.Services;
//using Market.Services.Interfaces;
//using Market.Tests.Helpers;
//using NUnit.Framework;
//using Serilog;

//namespace Market.Tests;

//public class AuthServiceTests : BaseRepositoryTests
//{
//    private AuthService _authService = null!;

//    [SetUp]
//    public void SetUp()
//    {
//        _authService = new AuthService(UnitOfWork, Log.Logger);
//    }


//    [Test]
//    public void Constructor_WhenUnitOfWorkIsNull_ShouldThrowArgumentNullException()
//    {
//        // Act and Assert
//        Assert.Throws<ArgumentNullException>(
//            () => new AuthService(null!, Log.Logger));
//    }

//    [Test]
//    public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()
//    {
//        // Act and Assert
//        Assert.Throws<ArgumentNullException>(
//            () => new AuthService(UnitOfWork, null!));
//    }

//    [Test]
//    public async Task Register_WithValidData_ShouldCreateAccount()
//    {
//        // Arrange
//        var username = "RegisterUser".AddGuid();
//        const string password = "SecurePassword123!";

//        // Act
//        await _authService.Register(username, password);

//        // Assert
//        var storedAccount = UnitOfWork.AccountRepository.GetByUsername(username);

//        Assert.That(storedAccount, Is.Not.Null);
//        Assert.Multiple(() =>
//        {
//            Assert.That(storedAccount!.Id, Is.GreaterThan(0));
//            Assert.That(storedAccount.Username, Is.EqualTo(username));
//            Assert.That(storedAccount.PasswordHash, Is.Not.Null.And.Not.Empty);
//            Assert.That(storedAccount.Hwid, Is.Not.Null.And.Not.Empty);
//            Assert.That(storedAccount.FailedLoginAttempts, Is.EqualTo(0));
//            Assert.That(storedAccount.LockoutTime, Is.Null);
//            Assert.That(storedAccount.IsDeleted, Is.False);
//        });
//    }

//    [Test]
//    public void Register_WithExistingUsername_ShouldThrowInvalidOperationException()
//    {
//        // Arrange
//        var duplicateUsername = "DuplicateUser".AddGuid();
//        var account = CreateSampleAccount(duplicateUsername, "Password123!");
//        UnitOfWork.AccountRepository.Insert(account);

//        // Act and Assert
//        var exception = Assert.ThrowsAsync<InvalidOperationException>(
//            async () => await _authService.Register(duplicateUsername, "AnotherPassword123!"));

//        Assert.That(exception!.Message, Is.EqualTo("Username already exists."));
//    }

//    [TestCase(null, "Password123!")]
//    [TestCase("", "Password123!")]
//    [TestCase("   ", "Password123!")]
//    [TestCase("ValidUser", null)]
//    [TestCase("ValidUser", "")]
//    [TestCase("ValidUser", "   ")]
//    public void Register_WhenCredentialsWhitespaceOrNull_ShouldThrowArgumentException(string? username, string? password)
//    {
//        // Act and Assert
//        Assert.ThrowsAsync<ArgumentException>(
//            async () => await _authService.Register(username!, password!));
//    }

//    [Test]
//    public async Task Login_WithValidCredentialsAndMatchingHwid_ShouldReturnSuccess()
//    {
//        // Arrange
//        var username = "ValidLoginUser".AddGuid();
//        const string password = "CorrectPassword123!";
//        var account = CreateSampleAccount(username, password);

//        var accountId = UnitOfWork.AccountRepository.Insert(account);

//        // Act
//        var result = await _authService.Login(username, password);

//        // Assert
//        var updatedAccount = UnitOfWork.AccountRepository.GetById(accountId);

//        Assert.Multiple(() =>
//        {
//            Assert.That(result.IsSuccess, Is.True);
//            Assert.That(result.UserId, Is.EqualTo(accountId));
//            Assert.That(result.ErrorMessage, Is.Null);
//            Assert.That(updatedAccount!.FailedLoginAttempts, Is.EqualTo(0));
//            Assert.That(updatedAccount.LockoutTime, Is.Null);
//        });
//    }

//    [Test]
//    public async Task Login_WhenUserDoesNotExist_ShouldReturnInvalidCredentials()
//    {
//        // Arrange
//        var missingUsername = "MissingUser".AddGuid();

//        // Act
//        var result = await _authService.Login(missingUsername, "AnyPassword123!");

//        // Assert
//        Assert.Multiple(() =>
//        {
//            Assert.That(result.IsSuccess, Is.False);
//            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid credentials."));
//        });
//    }

//    [Test]
//    public async Task Login_WithIncorrectPassword_ShouldIncrementFailedLoginAttempts()
//    {
//        // Arrange
//        var username = "WrongPasswordUser".AddGuid();
//        var account = CreateSampleAccount(username, "CorrectPassword123!");
//        var accountId = UnitOfWork.AccountRepository.Insert(account);

//        // Act
//        var result = await _authService.Login(username, "WrongPassword123!");

//        // Assert
//        var updatedAccount = UnitOfWork.AccountRepository.GetById(accountId);

//        Assert.Multiple(() =>
//        {
//            Assert.That(result.IsSuccess, Is.False);
//            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid credentials."));
//            Assert.That(updatedAccount!.FailedLoginAttempts, Is.EqualTo(1));
//            Assert.That(updatedAccount.LockoutTime, Is.Null);
//        });
//    }

//    [Test]
//    public async Task Login_WhenFailedAttemptsReachFive_ShouldLockAccountForTenMinutes()
//    {
//        // Arrange
//        var username = "LockoutUser".AddGuid();
//        var account = CreateSampleAccount(username, "CorrectPassword123!");
//        account.FailedLoginAttempts = 4; // Set to 4 so 1 failed attempt triggers lockout

//        var accountId = UnitOfWork.AccountRepository.Insert(account);

//        // Act
//        var result = await _authService.Login(username, "WrongPassword123!");

//        // Assert
//        var updatedAccount = UnitOfWork.AccountRepository.GetById(accountId);

//        Assert.Multiple(() =>
//        {
//            Assert.That(result.IsSuccess, Is.False);
//            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid credentials."));
//            Assert.That(updatedAccount!.FailedLoginAttempts, Is.EqualTo(5));
//            Assert.That(updatedAccount.LockoutTime, Is.Not.Null);
//            Assert.That(updatedAccount.LockoutTime!.Value, Is.GreaterThan(DateTime.UtcNow));
//        });
//    }

//    [Test]
//    public async Task Login_WhenAccountIsCurrentlyLocked_ShouldReturnLockoutMessage()
//    {
//        // Arrange
//        var username = "CurrentlyLockedUser".AddGuid();
//        var lockoutTime = DateTime.UtcNow.AddMinutes(10).ToUniversalTime();

//        var account = CreateSampleAccount(username, "CorrectPassword123!");
//        account.FailedLoginAttempts = 5;
//        account.LockoutTime = lockoutTime;

//        UnitOfWork.AccountRepository.Insert(account);
//        var storedAccount = UnitOfWork.AccountRepository.GetByUsername(username);


//        // Act
//        var result = await _authService.Login(username, "CorrectPassword123!");

//        // Assert
//        Assert.Multiple(() =>
//        {
//            Assert.That(result.IsSuccess, Is.False);
//            Assert.That(result.ErrorMessage, Is.EqualTo("Account is temporarily locked. Try again later."));
//        });
//    }

//    [Test]
//    public async Task Login_WhenHardwareFingerprintMismatches_ShouldReturnAccessDenied()
//    {
//        // Arrange
//        var username = "HwidMismatchUser".AddGuid();
//        const string password = "CorrectPassword123!";

//        var account = CreateSampleAccount(username, password, hwid: "DIFFERENT_HARDWARE_FINGERPRINT_123");
//        UnitOfWork.AccountRepository.Insert(account);

//        // Act
//        var result = await _authService.Login(username, password);

//        // Assert
//        Assert.Multiple(() =>
//        {
//            Assert.That(result.IsSuccess, Is.False);
//            Assert.That(result.ErrorMessage, Is.EqualTo("Access Denied: This account is registered to a different physical device."));
//        });
//    }

//    [TestCase(null, "Password123!")]
//    [TestCase("", "Password123!")]
//    [TestCase("   ", "Password123!")]
//    [TestCase("ValidUser", null)]
//    [TestCase("ValidUser", "")]
//    [TestCase("ValidUser", "   ")]

//    public void Login_WhenCredentialsWhitespaceOrNull_ShouldThrowArgumentException(string? username, string? password)
//    {
//        // Act and Assert
//        Assert.ThrowsAsync<ArgumentException>(
//            async () => await _authService.Login(username!, password!));
//    }

//    [Test]
//    public void Logout_ShouldCompleteWithoutExceptions()
//    {
//        // Act and Assert
//        Assert.DoesNotThrowAsync(async () => await _authService.Logout());
//    }

//    private static AccountDTO CreateSampleAccount(string username, string password, string? hwid = null)
//    {
//        return new AccountDTO
//        {
//            Username = username,
//            PasswordHash = PasswordHasher.HashPassword(password),
//            Email = $"user_{Guid.NewGuid():N}@test.com",
//            AccountType = 1,
//            FirstName = "John",
//            LastName = "Doe",
//            Hwid = hwid ?? HardwareFingerprintHelper.GenerateHardwareFingerprint(),
//            FailedLoginAttempts = 0,
//            LockoutTime = null,
//            IsDeleted = false
//        };
//    }
//}