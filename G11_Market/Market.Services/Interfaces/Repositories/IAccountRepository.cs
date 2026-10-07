using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IAccountRepository : IBaseRepository<AccountDTO>
{
    public AccountDTO? GetByUsername(string username);
    void UpdateLoginAttempts(
        int accountId,
        int failedLoginAttempts,
        DateTime? lastLoginAtUtc,
        DateTime? lockoutEndUtc);

    Task<IReadOnlyList<RoleDTO>> GetRolesByAccountIdAsync(
        int accountId,
        CancellationToken cancellationToken = default);
}
    public void UpdateLoginAttempts(int accountId, int failedAttempts, DateTime? lastLogin, DateTime? lockoutTime);
    public Task<AccountDTO?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    public Task RecordFailedLoginAsync(int accountId, DateTime failedAttemptTime, int maxFailedAttempts, TimeSpan lockoutDuration, CancellationToken cancellationToken = default);
    public Task RecordSuccessfulLoginAsync(int accountId, DateTime successfulAttemptTime, string? newPasswordHash, CancellationToken cancellationToken = default);
    public Task RefreshTokenAsync(int accountId, byte[] tokenHash, DateTime expiredAt, CancellationToken cancellationToken = default);
}
