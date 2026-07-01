using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

public sealed class CategoryRepository(DbConnection connection)
    : BaseRepository<CategoryDTO>(connection), ICategoryRepository
{
    public CategoryDTO? GetByName(string categoryName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryName);

        return Search(category =>
                category.CategoryName == categoryName &&
                category.IsDeleted == false)
            .FirstOrDefault();
    }

    public IEnumerable<CategoryDTO> GetRootCategories()
    {
        return Search(category =>
            category.ParentId == null &&
            category.IsDeleted == false);
    }

    public IEnumerable<CategoryDTO> GetChildren(int parentId)
    {
        if (parentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(parentId));

        return Search(category =>
            category.ParentId == parentId &&
            category.IsDeleted == false);
    }
}