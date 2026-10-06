using System.Data;
using System.Data.Common;
using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class AccountRepository(DbConnection connection) : BaseRepository<AccountDTO>(connection), IAccountRepository
{
    public AccountDTO GetByUsername(string username) => Search(a => a.Username == username).FirstOrDefault();
    public void UpdateLoginAttempts(int accountId, int failedAttempts, DateTime? lastLogin, DateTime? lockoutTime)
    {
        var account = GetById(accountId);
        if (account is null)
            throw new InvalidOperationException($"Account with ID {accountId} not found.");

        _connection.Execute(
            "sp_UpdateAccountLoginAttempts",
            new
            {
                Id = accountId,
                FailedLoginAttempts = failedAttempts,
                LockoutTime = lockoutTime
            },
            commandType: CommandType.StoredProcedure);
    }

    public Task<AccountDTO?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) => Task.FromResult(Search(a => a.Username == username).FirstOrDefault());
    public Task RecordFailedLoginAsync(int accountId, DateTime failedAttemptTime, int maxFailedAttempts, TimeSpan lockoutDuration, CancellationToken cancellationToken = default)
    {
        var account = GetById(accountId);
        if (account is null) throw new InvalidOperationException($"Account with ID {accountId} not found.");
        account.FailedLoginAttempts++;
        if (account.FailedLoginAttempts >= maxFailedAttempts)
        {
            account.LockoutEndUtc = failedAttemptTime.Add(lockoutDuration);
        }
        Update(account);
        return Task.CompletedTask;
    }
    public Task RecordSuccessfulLoginAsync(int accountId, DateTime successfulAttemptTime, string? newPasswordHash, CancellationToken cancellationToken = default)
    {
        var account = GetById(accountId);
        if (account is null) throw new InvalidOperationException($"Account with ID {accountId} not found.");
        account.FailedLoginAttempts = 0;
        account.LockoutEndUtc = null;
        account.LastLoginAtUtc = successfulAttemptTime;
        if (!string.IsNullOrEmpty(newPasswordHash))
        {
            account.PasswordHash = newPasswordHash;
        }
        Update(account);
        return Task.CompletedTask;
    }

    public Task RefreshTokenAsync(int accountId, byte[] tokenHash, DateTime expiredAt, CancellationToken cancellationToken = default)
    {
        if (accountId <= 0)
            throw new ArgumentOutOfRangeException(nameof(accountId));
        if (tokenHash == null || tokenHash.Length == 0)
            throw new ArgumentException("Token hash cannot be null or empty.", nameof(tokenHash));
        cancellationToken.ThrowIfCancellationRequested();
        _connection.Execute(
            "sp_RefreshToken",
            new
            {
                AccountId = accountId,
                Token = tokenHash,
                TokenExpiration = expiredAt
            },
            commandType: CommandType.StoredProcedure);
        return Task.CompletedTask;
    }
}