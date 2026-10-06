using Market.DTO;
using Market.Extensions;
using Market.Services;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Services;


public class AuthService
{

    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration =
        TimeSpan.FromMinutes(15);

    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly RefreshTokenService _refreshTokenService;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        RefreshTokenService refreshTokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<LoginResultDTO> Login(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            return FailedLogin("invalid credentials");
        }


        username = username.Trim();

        var user = await _unitOfWork.AccountRepository
            .GetByUsernameAsync(username, cancellationToken);

        if (user is null || user.IsDeleted)
        {
            return FailedLogin(
                "invalid credentials");
        }


        DateTime now = DateTime.UtcNow;

        var lockedTimeInfo = CheckLockedOutTime(user, cancellationToken);

        var verification = PasswordHasher.VerifyHashedPassword(
            user.PasswordHash,
            password);

        if (!verification)
        {

            await _unitOfWork.AccountRepository.RecordFailedLoginAsync(
                user.Id,
                now,
                MaxFailedAttempts,
                LockoutDuration,
                cancellationToken);

            return FailedLogin(
                "Input is invalid");
        }


        var roles = await _unitOfWork.RoleRepository
            .GetByAccountIdAsync(
                user.Id,
                cancellationToken);

        string? newPasswordHash = null;

        if (verification == true)
        {
            newPasswordHash = PasswordHasher.HashPassword(
                password);
        }


        string accessToken = _tokenService.GenerateToken(
            user,
            roles);


        await _unitOfWork.AccountRepository.RecordSuccessfulLoginAsync(
            user.Id,
            DateTime.UtcNow,
            newPasswordHash,
            cancellationToken);


        var refreshToken =
            await _refreshTokenService.CreateAndSaveAsync(
                user.Id,
                cancellationToken);


        return new LoginResultDTO
        {
            IsSuccess = true,
            Message = "AccountLoggedIn.",
            AccountId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiredAt = refreshToken.ExpiredAt
        };
    }

    private static LoginResultDTO FailedLogin(string message)
    {
        return new LoginResultDTO
        {
            IsSuccess = false,
            Message = message
        };
    }
    public Task CheckLockedOutTime(AccountDTO account, CancellationToken cancellationToken = default)
    {
        if (account.LockoutEndUtc.HasValue && account.LockoutEndUtc.Value > DateTime.UtcNow)
        {
            var remainingLockoutTime = account.LockoutEndUtc.Value - DateTime.UtcNow;
            throw new Exception($"Account is locked. Please try again after {remainingLockoutTime.TotalMinutes:F0} minutes.");
        }
        return Task.CompletedTask;
    }
}