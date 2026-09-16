using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IAttributeRepository : IBaseRepository<AttributeDTO>
{
    public AttributeDTO? GetByName(string name);
    public AttributeDTO? GetById(int id);
    public IEnumerable<AttributeDTO> GetByType(byte attributeType);
}