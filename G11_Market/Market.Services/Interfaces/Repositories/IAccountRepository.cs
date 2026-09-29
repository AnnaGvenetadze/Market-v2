using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IAccountRepository : IBaseRepository<AccountDTO>
{
    public AccountDTO? GetByUsername(string username);
    public AccountDTO? GetByEmail(string email);
    public IEnumerable<AccountDTO> GetByAccountType(byte accountType);
    public void UpdateLoginAttempts(int accountId, int failedAttempts, DateTime? lastLogin, DateTime? lockoutTime);
}
