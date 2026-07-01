using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

public sealed class ClientRepository(DbConnection connection)
    : BaseRepository<ClientDTO>(connection), IClientRepository
{
    public ClientDTO? GetByAccountId(int accountId)
        => Search(client => client.AccountId == accountId && client.IsDeleted == false)
            .FirstOrDefault();

    public IEnumerable<ClientDTO> GetByClientTypeId(int clientTypeId)
        => Search(client => client.ClientTypeId == clientTypeId && client.IsDeleted == false);

    public ClientDTO? GetByPhoneNumber(string phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

        return Search(client => client.PhoneNumber == phoneNumber && client.IsDeleted == false)
            .FirstOrDefault();
    }

    public IEnumerable<ClientDTO> GetAllActive()
        => Search(client => client.IsDeleted == false);
}