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
}