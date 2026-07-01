using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface ICategoryRepository : IBaseRepository<CategoryDTO>
{
    CategoryDTO? GetByName(string categoryName);
    IEnumerable<CategoryDTO> GetRootCategories();
    IEnumerable<CategoryDTO> GetChildren(int parentId);
}