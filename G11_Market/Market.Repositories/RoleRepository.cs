using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

public sealed class RoleRepository(DbConnection connection)
    : BaseRepository<RoleDTO>(connection), IRoleRepository
{
    public RoleDTO GetByName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return Search(role =>
                role.Name == name &&
                role.IsDeleted == false)
            .FirstOrDefault();
    }
}