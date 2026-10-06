using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IRoleRepository : IBaseRepository<RoleDTO>
{
    RoleDTO? GetByName(string name);
    Task<IReadOnlyList<RoleDTO>> GetByAccountIdAsync(
          int accountId,
          CancellationToken cancellationToken = default);
}
