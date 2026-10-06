using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IAccountRepository : IBaseRepository<AccountDTO>
{
    public AccountDTO? GetByUsername(string username);
    public void UpdateLoginAttempts(int accountId, int failedAttempts, DateTime? lastLogin, DateTime? lockoutTime);
    public Task<AccountDTO?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    public Task RecordFailedLoginAsync(int accountId, DateTime failedAttemptTime, int maxFailedAttempts, TimeSpan lockoutDuration, CancellationToken cancellationToken = default);
    public Task RecordSuccessfulLoginAsync(int accountId, DateTime successfulAttemptTime, string? newPasswordHash, CancellationToken cancellationToken = default);
    public Task RefreshTokenAsync(int accountId, byte[] tokenHash, DateTime expiredAt, CancellationToken cancellationToken = default);
}
