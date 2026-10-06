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
}