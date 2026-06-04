using Market.DTO;

namespace Market.Repositories.Interfaces;

public interface IAccountRepository
{
    public AccountDTO GetByUsername(string username);
    public AccountDTO GetByEmail(string email);
    public IEnumerable<AccountDTO> GetByAccountType(byte accountType);
}
