using System.Data;
using System.Data.Common;
using Dapper;
using Market.DTO;
using Market.Repositories;
using Market.Services.Interfaces.Repositories;

internal sealed class AccountRepository(DbConnection connection, Func<DbTransaction?> transaction)
    : BaseRepository<AccountDTO>(connection, transaction), IAccountRepository
{
    public AccountDTO? GetByUsername(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        return Search(a => a.Username == username.Trim())
            .FirstOrDefault();
    }


    public void UpdateLoginAttempts(
        int accountId,
        int failedLoginAttempts,
        DateTime? lastLoginAtUtc,
        DateTime? lockoutEndUtc)
    {
        if (accountId <= 0)
            throw new ArgumentOutOfRangeException(nameof(accountId));

        if (failedLoginAttempts < 0)
            throw new ArgumentOutOfRangeException(nameof(failedLoginAttempts));

        var account = GetById(accountId);

        if (account is null)
            throw new InvalidOperationException(
                $"Account with ID {accountId} not found.");

        _connection.Execute(
            "sp_UpdateAccountLoginAttempts",
            new
            {
                AccountId = accountId,
                FailedLoginAttempts = failedLoginAttempts,
                LastLoginAtUtc = lastLoginAtUtc,
                LockoutEndUtc = lockoutEndUtc
            },
            transaction: _transaction(),
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
            transaction: _transaction(),
            commandType: CommandType.StoredProcedure);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<RoleDTO>> GetRolesByAccountIdAsync(
        int accountId,
        CancellationToken cancellationToken = default)
    {
        if (accountId <= 0)
            throw new ArgumentOutOfRangeException(nameof(accountId));

        var command = new CommandDefinition(
            commandText: "dbo.sp_GetRolesByAccountId",
            parameters: new { AccountId = accountId },
            commandType: CommandType.StoredProcedure,
            transaction: _transaction(),
            cancellationToken: cancellationToken);

        var roles = await _connection.QueryAsync<RoleDTO>(command);

        return roles.ToList();
    }

    public void ResetPassword(int accountId, string newPasswordHash)
    {
        if (accountId <= 0)
            throw new ArgumentOutOfRangeException(nameof(accountId));

        ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);

        int rowsAffected = _connection.Execute(
            "sp_ResetAccountPassword",
            new
            {
                AccountId = accountId,
                NewPasswordHash = newPasswordHash
            },
            transaction: _transaction(),
            commandType: CommandType.StoredProcedure);

        if (rowsAffected == 0)
        {
            throw new KeyNotFoundException($"Account with ID {accountId} was not found or is deleted.");
        }
    }
}

