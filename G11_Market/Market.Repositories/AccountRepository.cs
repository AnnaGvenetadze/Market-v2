using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class AccountRepository(DbConnection connection) : BaseRepository<AccountDTO>(connection), IAccountRepository
{
    public AccountDTO GetByUsername(string username) => Search(a => a.Username == username).FirstOrDefault();
    public AccountDTO GetByEmail(string email) => Search(a => a.Email == email).FirstOrDefault();
    public IEnumerable<AccountDTO> GetByAccountType(byte accountType) => Search(a => a.AccountType == accountType);
}