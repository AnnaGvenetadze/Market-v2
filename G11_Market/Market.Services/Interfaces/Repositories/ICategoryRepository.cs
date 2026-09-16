using System.Data;
using System.Data.Common;
using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface ICategoryRepository : IBaseRepository<CategoryDTO>
{
    public CategoryDTO? GetByName(string categoryName);
    public IEnumerable<CategoryDTO> GetRootCategories();
    public IEnumerable<CategoryDTO> GetDeletedCategories();
    public IEnumerable<CategoryDTO> GetChildren(int parentId);
    public void AssignAttribute(CategoryAttributeDTO categoryAttribute);
    public void UnassignAttribute(CategoryAttributeDTO categoryAttribute);
}