using System.Data;
using System.Data.Common;
using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class CategoryRepository(DbConnection connection, Func<DbTransaction?> transaction)
    : BaseRepository<CategoryDTO>(connection, transaction), ICategoryRepository
{
    public CategoryDTO? GetByName(string categoryName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryName);

        return Search(category =>
                category.CategoryName == categoryName)
            .FirstOrDefault();
    }

    public IEnumerable<CategoryDTO> GetRootCategories()
    {
        return Search(category =>
            category.ParentId == null);
    }

    public IEnumerable<CategoryDTO> GetDeletedCategories()
    {
        return Search(category =>
            category.IsDeleted == true);
    }

    public IEnumerable<CategoryDTO> GetChildren(int parentId)
    {
        if (parentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(parentId));

        return Search(category =>
            category.ParentId == parentId);
    }

    public void AssignAttribute(CategoryAttributeDTO categoryAttribute)
    {
        _connection.Execute(
            "sp_AssignCategoryAttribute",
            new
            {
                CategoryId = categoryAttribute.CategoryId,
                AttributeId = categoryAttribute.AttributeId,
                OrderPosition = categoryAttribute.OrderPosition
            },
            transaction: _transaction(),
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
            transaction: _transaction(),
            commandType: CommandType.StoredProcedure);
    }
}