using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

// TODO: ამის ესაინ ანესაინი როგორ? სუფთად კაშირის თეიბლი არაა
internal sealed class ProductAttributeValueRepository(DbConnection connection)
    : CompositeKeyBaseRepository<ProductAttributeValueDTO>(connection), IProductAttributeValueRepository
{
    protected override string FirstKeyName => "ProductId";
    protected override string SecondKeyName => "AttributeId";
}