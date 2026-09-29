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

    public async Task<LoginResultDTO> Login(string username, string password, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        DateTime now = DateTime.UtcNow;

        var user = await GetUserByUsernameAsync(username, cancellationToken);
        if (user is null)
            return FailedLogin("Invalid credentials.");

        if (IsAccountLocked(user, now))
            return FailedLogin("Account is temporarily locked. Try again later.");

        if (!PasswordHasher.VerifyHashedPassword(password, user.PasswordHash))
            return await FailedLoginAsync(user, now, cancellationToken);

        if (!ValidateHardwareFingerprint(user))
            return FailedLogin("Access Denied: This account is registered to a different physical device.");

        return await SuccessfulLoginAsync(user, now, cancellationToken);
    }

    private async Task<AccountDTO?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var user = _unitOfWork
            .AccountRepository
            .Search(a => a.Username == username && !a.IsDeleted)
            .FirstOrDefault();

        if (user is null)
            return null;

        return new AccountDTO
        {
            Id = user.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            Hwid = user.Hwid,
            FailedLoginAttempts = user.FailedLoginAttempts,
            LockoutTime = user.LockoutTime
        };
    }

    private bool IsAccountLocked(AccountDTO user, DateTime now)
    {
        if (!user.LockoutTime.HasValue)
            return false;

        if (user.LockoutTime > now)
        {
            _logger.Warning("Login denied for locked user {UserId} until {LockoutTime}", user.Id, user.LockoutTime);
            return true;
        }

        user.LockoutTime = null;
        user.FailedLoginAttempts = 0;
        return false;
    }

    private async Task<LoginResultDTO> FailedLoginAsync(AccountDTO user, DateTime now, CancellationToken cancellationToken)
    {
        int failedAttempts = user.FailedLoginAttempts + 1;
        DateTime? lockoutTime = null;

        if (failedAttempts >= 5)
        {
            lockoutTime = now.AddMinutes(10);
            _logger.Warning("User {UserId} exceeded max failed attempts. Locked out until {LockoutTime}", user.Id, lockoutTime);
        }

        _unitOfWork.AccountRepository.UpdateLoginAttempts(user.Id, failedAttempts, now, lockoutTime);

        return FailedLogin("Invalid credentials.");
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

    private async Task<LoginResultDTO> SuccessfulLoginAsync(AccountDTO user, DateTime now, CancellationToken cancellationToken)
    {
        _unitOfWork.AccountRepository.UpdateLoginAttempts(user.Id, 0, now, null);

        _logger.Information("User {UserId} logged in successfully at {Time}.", user.Id, now);

        return new LoginResultDTO
        {
            IsSuccess = true,
            UserId = user.Id
        };
    }

    private static LoginResultDTO FailedLogin(string errorMessage)
    {
        return new LoginResultDTO
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}