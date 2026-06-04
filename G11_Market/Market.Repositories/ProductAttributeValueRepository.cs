using System.Data.Common;
using Market.DTO;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public sealed class ProductAttributeValueRepository(DbConnection connection)
    : CompositeKeyBaseRepository<ProductAttributeValueDTO>(connection), IProductAttributeValueRepository
{
    protected override string FirstKeyName => "ProductId";
    protected override string SecondKeyName => "AttributeId";
}