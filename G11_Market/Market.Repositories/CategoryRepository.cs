using System.Data;
using System.Data.Common;
using Dapper;
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

    public void AssignAttribute(CategoryAttributeDTO categoryAttribute)
    {
        _connection.Execute(
            "sp_AssignCategoryAttribute",
            new
            {
                CategoryId = categoryAttribute.CategoryId, 
                AttributeId = categoryAttribute.AttributeId
            },
            commandType: CommandType.StoredProcedure);
    }

    public void UnassignAttribute(CategoryAttributeDTO categoryAttribute)
    {
        _connection.Execute(
            "sp_UnassignCategoryAttribute",
            new
            {
                CategoryId = categoryAttribute.CategoryId, 
                AttributeId = categoryAttribute.AttributeId
            },
            commandType: CommandType.StoredProcedure);
    }
}