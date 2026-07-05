using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

// TODO: წასაშლელია ეს რეპო (შუალედური ცხრილებისთვის ცალკე არც რეპოა არც ტესტი)
internal sealed class CategoryAttributeRepository(DbConnection connection)
    : CompositeKeyBaseRepository<CategoryAttributeDTO>(connection), ICategoryAttributeRepository
{
    protected override string FirstKeyName => "CategoryId";
    protected override string SecondKeyName => "AttributeId";
}