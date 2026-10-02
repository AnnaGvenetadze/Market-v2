using Market.DTO;
using Market.Extensions;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Services;
using Serilog;

namespace Market.Services;

public class AuthService : IAuthService
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUnitOfWork unitOfWork, ILogger logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AuthResultDTO> Login(string username, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or whitespace.", nameof(username));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or whitespace.", nameof(password));

        DateTime now = DateTime.UtcNow;

        var user = await GetUserByUsernameAsync(username, cancellationToken);
        if (user is null)
            return FailedLogin("Invalid credentials.");

        if (IsAccountLocked(user, now))
            return FailedLogin("Account is temporarily locked. Try again later.");

        if (!PasswordHasher.VerifyHashedPassword(user.PasswordHash, password))
            return await FailedLoginAsync(user, now, cancellationToken);

        if (!ValidateHardwareFingerprint(user))
            return FailedLogin("Access Denied: This account is registered to a different physical device.");

        return await SuccessfulLoginAsync(user, now, cancellationToken);
    }

    public async Task Logout(CancellationToken cancellationToken = default)
    {
        _logger.Information("User logged out at {Time}.", DateTime.UtcNow);
        await Task.CompletedTask;
    }

    public async Task Register(string username, string password, string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or whitespace.", nameof(username));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or whitespace.", nameof(password));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or whitespace.", nameof(email));

        var existingUser = await GetUserByUsernameAsync(username, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException("Username already exists.");

        string hashedPassword = PasswordHasher.HashPassword(password);
        string hwid = HardwareFingerprintHelper.GenerateHardwareFingerprint();

        var newUser = new AccountDTO
        {
            Username = username,
            PasswordHash = hashedPassword,
            Email = email,
            FirstName = username,
            LastName = "User",
            AccountType = 1,
            Hwid = hwid,
            FailedLoginAttempts = 0,
            LockoutTime = null,
            IsDeleted = false
        };

        _unitOfWork.AccountRepository.Insert(newUser);
        _logger.Information("New user registered: {Username} at {Time}.", username, DateTime.UtcNow);
        await Task.CompletedTask;
    }

    private async Task<AccountDTO?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var user = _unitOfWork.AccountRepository.GetByUsername(username);

        if (user is null || user.IsDeleted)
            return null;

        return user;
    }

    private bool IsAccountLocked(AccountDTO user, DateTime now)
    {
        if (!user.LockoutTime.HasValue)
            return false;

        DateTime lockoutTime = user.LockoutTime.Value;
        if (lockoutTime.Kind == DateTimeKind.Unspecified)
        {
            lockoutTime = DateTime.SpecifyKind(lockoutTime, DateTimeKind.Utc);
        }

        if (lockoutTime > now)
        {
            _logger.Warning("Login denied for locked user {UserId} until {LockoutTime}", user.Id, user.LockoutTime);
            return true;
        }

        user.LockoutTime = null;
        user.FailedLoginAttempts = 0;
        return false;
    }

    private async Task<AuthResultDTO> FailedLoginAsync(AccountDTO user, DateTime now, CancellationToken cancellationToken)
    {
        int failedAttempts = user.FailedLoginAttempts + 1;
        DateTime? lockoutTime = null;

        if (failedAttempts >= 5)
        {
            lockoutTime = now.AddMinutes(10);
            _logger.Warning("User {UserId} exceeded max failed attempts. Locked out until {LockoutTime}", user.Id, lockoutTime);
        }

        _unitOfWork.AccountRepository.UpdateLoginAttempts(user.Id, failedAttempts, now, lockoutTime);

        return await Task.FromResult(FailedLogin("Invalid credentials."));
    }

    private bool ValidateHardwareFingerprint(AccountDTO user)
    {
        string currentHwid = HardwareFingerprintHelper.GenerateHardwareFingerprint();

        if (!string.Equals(user.Hwid, currentHwid, StringComparison.OrdinalIgnoreCase))
        {
            _logger.Warning("Hardware mismatch for User {UserId}. Registered HWID: {RegisteredHwid}, Current HWID: {CurrentHwid}",
                user.Id, user.Hwid, currentHwid);
            return false;
        }

        return true;
    }

    private async Task<AuthResultDTO> SuccessfulLoginAsync(AccountDTO user, DateTime now, CancellationToken cancellationToken)
    {
        _unitOfWork.AccountRepository.UpdateLoginAttempts(user.Id, 0, now, null);

        _logger.Information("User {UserId} logged in successfully at {Time}.", user.Id, now);

        return await Task.FromResult(new AuthResultDTO
        {
            IsSuccess = true,
            UserId = user.Id
        });
    }

    private static AuthResultDTO FailedLogin(string errorMessage)
    {
        return new AuthResultDTO
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}