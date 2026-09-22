using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class ClientTypeRepository(DbConnection connection)
    : BaseRepository<ClientTypeDTO>(connection), IClientTypeRepository
{
    public ClientTypeDTO? GetByName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var trimmedName = name.Trim();
        return Search(type => type.Name == trimmedName)
            .FirstOrDefault();
    }

    public IEnumerable<ClientTypeDTO> GetDeletedClientTypes()
    {
        return Search(type => type.IsDeleted == true);
    }
}