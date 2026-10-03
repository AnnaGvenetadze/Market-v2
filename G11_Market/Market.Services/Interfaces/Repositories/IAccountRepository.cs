using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IAccountRepository : IBaseRepository<AccountDTO>
{
    public AccountDTO? GetByUsername(string username);
    public void UpdateLoginAttempts(int accountId, int failedAttempts, DateTime? lastLogin, DateTime? lockoutTime);
}
