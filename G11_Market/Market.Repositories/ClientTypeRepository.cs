using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class ClientTypeRepository(DbConnection connection)
    : BaseRepository<ClientTypeDTO>(connection), IClientTypeRepository
{
}
