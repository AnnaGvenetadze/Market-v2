using System.Data;
using System.Data.Common;
using Market.DTO;
using Market.Repositories;
using Market.Services.Interfaces.Repositories;
using Dapper;

internal sealed class AccountRepository(DbConnection connection)
    : BaseRepository<AccountDTO>(connection), IAccountRepository
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
            commandType: CommandType.StoredProcedure);
    }
}
