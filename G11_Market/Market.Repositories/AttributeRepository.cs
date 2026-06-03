using Market.DTO;
using Market.Repositories.Interfaces;
using System.Data.Common;

namespace Market.Repositories;

public class AttributeRepository(DbConnection connection)
    : BaseRepository<AttributeDTO>(connection), IAttributeRepository
{
}
