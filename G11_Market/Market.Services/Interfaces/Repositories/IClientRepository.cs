using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IClientRepository : IBaseRepository<ClientDTO>
{
    public ClientDTO? GetByAccountId(int accountId);
    public IEnumerable<ClientDTO> GetByClientTypeId(int clientTypeId);
    public ClientDTO? GetByPhoneNumber(string phoneNumber);
    public IEnumerable<ClientDTO> GetAllActive();
}