using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IClientRepository : IBaseRepository<ClientDTO>
{
    ClientDTO? GetByAccountId(int accountId);
    ClientDTO? GetByPhoneNumber(string phoneNumber);
    ClientDTO? GetByContactEmail(string contactEmail);
    IEnumerable<ClientDTO> GetByClientTypeId(int clientTypeId);
    IEnumerable<ClientDTO> GetDeletedClients();
}