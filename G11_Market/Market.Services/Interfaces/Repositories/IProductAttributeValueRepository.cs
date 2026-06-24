using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IProductAttributeValueRepository
    : ICompositeKeyBaseRepository<ProductAttributeValueDTO>
{
}