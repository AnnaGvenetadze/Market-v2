using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

internal class AttributeRepository(DbConnection connection)
    : BaseRepository<AttributeDTO>(connection), IAttributeRepository
{
}
