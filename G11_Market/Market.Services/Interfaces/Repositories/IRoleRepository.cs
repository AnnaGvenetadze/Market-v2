using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IRoleRepository : IBaseRepository<RoleDTO>
{
    RoleDTO? GetByName(string name);
}
