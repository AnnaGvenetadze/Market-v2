using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class ClientRepository(DbConnection connection)
    : BaseRepository<ClientDTO>(connection), IClientRepository
{
    public ClientDTO? GetByAccountId(int accountId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(accountId);

        return Search(client => client.AccountId == accountId)
            .FirstOrDefault();
    }

    public ClientDTO? GetByPhoneNumber(string phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);

        var trimmedPhone = phoneNumber.Trim();

        return Search(client => client.PhoneNumber == trimmedPhone)
            .FirstOrDefault();
    }

    public ClientDTO? GetByContactEmail(string contactEmail)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contactEmail);

        var trimmedEmail = contactEmail.Trim();

        return Search(client => client.ContactEmail == trimmedEmail)
            .FirstOrDefault();
    }

    public IEnumerable<ClientDTO> GetByClientTypeId(int clientTypeId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(clientTypeId);

        return Search(client => client.ClientTypeId == clientTypeId);
    }

    public IEnumerable<ClientDTO> GetDeletedClients()
    {
        return Search(client => client.IsDeleted == true);
    }
}