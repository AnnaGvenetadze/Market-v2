using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Market.Tests;

[TestFixture]
public class CategoryAttributeRepositoryTests : BaseRepositoryTests
{
    private int _testCategoryId;
    private int _testAttributeId;

    [SetUp]
    public void SetUp()
    {
        // Setup valid parent records for foreign key requirements
        var category = new CategoryDTO
        {
            CategoryName = "Category_".AddGuid(),
            Description = "Test Category"
        };
        _testCategoryId = UnitOfWork.CategoryRepository.Insert(category);

        var attribute = new AttributeDTO
        {
            AttributeName = "Attribute_".AddGuid()
        };
        _testAttributeId = UnitOfWork.AttributeRepository.Insert(attribute);
    }

    [Test]
    public void Insert_ValidCompositeMapping_ShouldSucceed()
    {
        var mapping = new CategoryAttributeDTO
        {
            CategoryId = _testCategoryId,
            AttributeId = _testAttributeId,
            OrderPosition = 1
        };

        Assert.DoesNotThrow(() => UnitOfWork.CategoryAttributeRepository.Insert(mapping));

        var results = UnitOfWork.CategoryAttributeRepository.GetById(_testCategoryId).ToList();
        Assert.That(results.Any(x => x.AttributeId == _testAttributeId), Is.True);
    }

    [Test]
    public void Insert_NullEntity_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.CategoryAttributeRepository.Insert(null!));
    }

    [Test]
    public void Insert_DuplicateCompositeKey_ShouldThrowSqlException()
    {
        var mapping = new CategoryAttributeDTO
        {
            CategoryId = _testCategoryId,
            AttributeId = _testAttributeId,
            OrderPosition = 1
        };

        UnitOfWork.CategoryAttributeRepository.Insert(mapping);

        Assert.Throws<SqlException>(() =>
            UnitOfWork.CategoryAttributeRepository.Insert(mapping));
    }

    [Test]
    public void GetById_ByFirstKey_ShouldReturnAllAttributesForCategory()
    {
        var secondAttributeId = UnitOfWork.AttributeRepository.Insert(new AttributeDTO { AttributeName = "Attr2_".AddGuid() });

        UnitOfWork.CategoryAttributeRepository.Insert(new CategoryAttributeDTO { CategoryId = _testCategoryId, AttributeId = _testAttributeId, OrderPosition = 1 });
        UnitOfWork.CategoryAttributeRepository.Insert(new CategoryAttributeDTO { CategoryId = _testCategoryId, AttributeId = secondAttributeId, OrderPosition = 2 });

        var results = UnitOfWork.CategoryAttributeRepository.GetById(_testCategoryId).ToList();

        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(results.All(x => x.CategoryId == _testCategoryId), Is.True);
    }

    [Test]
    public void GetById_NullFirstKey_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.CategoryAttributeRepository.GetById(null!));
    }

    [Test]
    public void Delete_ValidCompositeKeys_ShouldRemoveMapping()
    {
        var mapping = new CategoryAttributeDTO
        {
            CategoryId = _testCategoryId,
            AttributeId = _testAttributeId,
            OrderPosition = 1
        };
        UnitOfWork.CategoryAttributeRepository.Insert(mapping);

        UnitOfWork.CategoryAttributeRepository.Delete(_testCategoryId, _testAttributeId);

        var results = UnitOfWork.CategoryAttributeRepository.GetById(_testCategoryId);
        Assert.That(results.Any(x => x.AttributeId == _testAttributeId), Is.False);
    }

    [Test]
    public void Delete_NullKeys_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.CategoryAttributeRepository.Delete(null!, _testAttributeId));

        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.CategoryAttributeRepository.Delete(_testCategoryId, null!));
    }
}