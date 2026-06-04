using Market.DTO;
using Market.Repositories.Interfaces;
using System.Data.Common;

namespace Market.Repositories;
public class CategoryRepository(DbConnection connection)
    : BaseRepository<CategoryDTO>(connection), ICategoryRepository
{
}
