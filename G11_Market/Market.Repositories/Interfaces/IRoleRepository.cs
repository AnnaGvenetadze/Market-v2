using Market.DTO;

namespace Market.Repositories.Interfaces;

public interface IRoleRepository : IBaseRepository<RoleDTO>
{
    RoleDTO GetByName(string name);
}
