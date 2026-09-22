using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IClientTypeRepository : IBaseRepository<ClientTypeDTO>
{
    ClientTypeDTO? GetByName(string name);
    IEnumerable<ClientTypeDTO> GetDeletedClientTypes();
}