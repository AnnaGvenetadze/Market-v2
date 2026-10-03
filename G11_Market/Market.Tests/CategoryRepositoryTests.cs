using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Market.Tests;

[TestFixture]
public class CategoryRepositoryTests : BaseRepositoryTests
{
    private static CategoryDTO CreateValidCategory(int? parentId = null) => new()
    {
        ParentId = parentId,
        CategoryName = "Category_".AddGuid(),
        Description = "Sample description"
    };

    [Test]
    public void Insert_ValidCategory_ShouldReturnNewId()
    {
        var category = CreateValidCategory();

        var newId = UnitOfWork.CategoryRepository.Insert(category);
        var inserted = UnitOfWork.CategoryRepository.GetById(newId);

        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(inserted, Is.Not.Null);
        Assert.That(inserted!.CategoryName, Is.EqualTo(category.CategoryName));
    }

    [Test]
    public void Insert_NullEntity_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CategoryRepository.Insert((CategoryDTO)null!));
    }

    [Test]
    public void Insert_DuplicateName_ShouldThrowSqlException()
    {
        var category1 = CreateValidCategory();
        UnitOfWork.CategoryRepository.Insert(category1);

        var category2 = CreateValidCategory();
        category2.CategoryName = category1.CategoryName;

        Assert.Throws<SqlException>(() => UnitOfWork.CategoryRepository.Insert(category2));
    }

    [Test]
    public void GetById_WhenExists_ShouldReturnCategory()
    {
        var category = CreateValidCategory();
        var id = UnitOfWork.CategoryRepository.Insert(category);

        var result = UnitOfWork.CategoryRepository.GetById(id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }

    [Test]
    public void GetById_WhenNotFound_ShouldReturnNull()
    {
        var result = UnitOfWork.CategoryRepository.GetById(-1);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetByName_ValidName_ShouldReturnCategory()
    {
        var category = CreateValidCategory();
        UnitOfWork.CategoryRepository.Insert(category);

        var result = UnitOfWork.CategoryRepository.GetByName(category.CategoryName);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CategoryName, Is.EqualTo(category.CategoryName));
    }

    [Test]
    public void GetByName_NullOrWhitespace_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CategoryRepository.GetByName(null!));
        Assert.Throws<ArgumentException>(() => UnitOfWork.CategoryRepository.GetByName("   "));
    }

    [Test]
    public void GetRootCategories_ShouldReturnCategoriesWithNullParent()
    {
        var root = CreateValidCategory();
        var rootId = UnitOfWork.CategoryRepository.Insert(root);

        var child = CreateValidCategory(rootId);
        UnitOfWork.CategoryRepository.Insert(child);

        var roots = UnitOfWork.CategoryRepository.GetRootCategories().ToList();

        Assert.That(roots.Any(c => c.Id == rootId), Is.True);
        Assert.That(roots.All(c => c.ParentId == null), Is.True);
    }

    [Test]
    public void GetChildren_ValidParentId_ShouldReturnSubCategories()
    {
        var parent = CreateValidCategory();
        var parentId = UnitOfWork.CategoryRepository.Insert(parent);

        var child1 = CreateValidCategory(parentId);
        var child2 = CreateValidCategory(parentId);
        UnitOfWork.CategoryRepository.Insert(child1);
        UnitOfWork.CategoryRepository.Insert(child2);

        var children = UnitOfWork.CategoryRepository.GetChildren(parentId).ToList();

        Assert.That(children.Count, Is.GreaterThanOrEqualTo(2));
        Assert.That(children.All(c => c.ParentId == parentId), Is.True);
    }

    [Test]
    public void GetChildren_InvalidParentId_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => UnitOfWork.CategoryRepository.GetChildren(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => UnitOfWork.CategoryRepository.GetChildren(-1));
    }

    [Test]
    public void Update_ValidData_ShouldModifyCategory()
    {
        var category = CreateValidCategory();
        var id = UnitOfWork.CategoryRepository.Insert(category);
        var inserted = UnitOfWork.CategoryRepository.GetById(id)!;

        inserted.CategoryName = "Updated_".AddGuid();
        inserted.Description = "Updated Desc";

        UnitOfWork.CategoryRepository.Update(inserted);
        var updated = UnitOfWork.CategoryRepository.GetById(id);

        Assert.That(updated!.CategoryName, Is.EqualTo(inserted.CategoryName));
        Assert.That(updated.Description, Is.EqualTo("Updated Desc"));
    }

    [Test]
    public void Update_SelfReferencingParent_ShouldThrowSqlException()
    {
        var category = CreateValidCategory();
        var id = UnitOfWork.CategoryRepository.Insert(category);
        var inserted = UnitOfWork.CategoryRepository.GetById(id)!;

        inserted.ParentId = id;

        Assert.Throws<SqlException>(() => UnitOfWork.CategoryRepository.Update(inserted));
    }

    [Test]
    public void Delete_ValidId_ShouldSoftDelete()
    {
        var category = CreateValidCategory();
        var id = UnitOfWork.CategoryRepository.Insert(category);

        UnitOfWork.CategoryRepository.Delete(id);

        var result = UnitOfWork.CategoryRepository.GetById(id);
        Assert.That(result, Is.Null);

        var deletedCategories = UnitOfWork.CategoryRepository.GetDeletedCategories();
        Assert.That(deletedCategories.Any(c => c.Id == id), Is.True);
    }

    [Test]
    public void AssignAndUnassignAttribute_ShouldExecuteSuccessfully()
    {
        var category = CreateValidCategory();
        var categoryId = UnitOfWork.CategoryRepository.Insert(category);

        var attribute = new AttributeDTO
        {
            AttributeName = "Attr_".AddGuid(),
            AttributeType = 1
        };
        var attributeId = UnitOfWork.AttributeRepository.Insert(attribute);

        Assert.That(attributeId, Is.GreaterThan(0), "Attribute Insert failed to return a valid ID. Check sp_InsertAttribute output parameter.");

        var mapping = new CategoryAttributeDTO
        {
            CategoryId = categoryId,
            AttributeId = attributeId,
            OrderPosition = 1
        };

        UnitOfWork.CategoryRepository.AssignAttribute(mapping);
        UnitOfWork.CategoryRepository.UnassignAttribute(mapping);
    }

    [Test]
    public void Restore_ShouldRestoreDeletedCategory()
    {
        // Arrange
        UnitOfWork.CategoryRepository.Delete(DeleteTestId);

        // Act
        UnitOfWork.CategoryRepository.Restore(DeleteTestId);

        // Assert
        var category = UnitOfWork.CategoryRepository.GetById(DeleteTestId);

        Assert.That(category, Is.Not.Null);
        Assert.That(category!.IsDeleted, Is.False);
    }

    [Test]
    public void Restore_WhenCategoryDoesNotExist_ShouldThrow()
    {
        Assert.Throws<SqlException>(() =>
            UnitOfWork.CategoryRepository.Restore(int.MaxValue));
    }

    [Test]
    public void Restore_WhenCategoryIsNotDeleted_ShouldThrow()
    {
        Assert.Throws<SqlException>(() =>
            UnitOfWork.CategoryRepository.Restore(1));
    }
}