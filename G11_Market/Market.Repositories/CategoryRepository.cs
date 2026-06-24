using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

public sealed class CategoryRepository(DbConnection connection)
    : BaseRepository<CategoryDTO>(connection), ICategoryRepository
{
}
