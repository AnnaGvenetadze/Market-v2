using System.Data.Common;
using Market.DTO;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public sealed class CategoryAttributeRepository(DbConnection connection)
    : CompositeKeyBaseRepository<CategoryAttributeDTO>(connection), ICategoryAttributeRepository
{
    protected override string FirstKeyName => "CategoryId";
    protected override string SecondKeyName => "AttributeId";
}