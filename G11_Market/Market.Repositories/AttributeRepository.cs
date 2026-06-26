using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

public class AttributeRepository(DbConnection connection)
    : BaseRepository<AttributeDTO>(connection), IAttributeRepository
{
}
