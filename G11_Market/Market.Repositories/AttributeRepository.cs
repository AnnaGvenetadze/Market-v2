using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class AttributeRepository(DbConnection connection)
    : BaseRepository<AttributeDTO>(connection), IAttributeRepository
{
    public AttributeDTO? GetByName(string name) => Search(a => a.AttributeName == name).FirstOrDefault();
    public AttributeDTO? GetById(int id) => Search(a => a.Id == id).FirstOrDefault();
    public IEnumerable<AttributeDTO> GetByType(byte attributeType) => Search(a => a.AttributeType == attributeType);
}
