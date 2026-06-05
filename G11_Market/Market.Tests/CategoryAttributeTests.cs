using Market.DTO;
using Market.Repositories;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class CategoryAttributeRepositoryTests
{
    private const string ConnectionString = "Server=localhost;Database=MarketDB_Test;Trusted_Connection=True;TrustServerCertificate=True;";
    private SqlConnection _connection;
    private CategoryRepository _categoryRepository;
    private AttributeRepository _attributeRepository;
    private CategoryAttributeRepository _categoryAttributeRepository;
    private int _categoryId;
    private int _attributeId;

    [SetUp]
    public void Setup()
    {
        _connection = new SqlConnection(ConnectionString);
        _categoryRepository = new CategoryRepository(_connection);
        _attributeRepository = new AttributeRepository(_connection);
        _categoryAttributeRepository = new CategoryAttributeRepository(_connection);

        var category = new CategoryDTO
        {
            CategoryName = TestDataHelper.AddGuid("TestCategory"),
            Description = "Test category description"
        };
        var attribute = new AttributeDTO
        {
            AttributeName = TestDataHelper.AddGuid("TestAttributeName"),
            AttributeType = 1
        };

        _categoryId = _categoryRepository.Insert(category);
        _attributeId = _attributeRepository.Insert(attribute);
    }


    [TearDown]
    public void TearDown()
    {
        _categoryRepository.Dispose();
        _attributeRepository.Dispose();
        _connection.Dispose();
    }


    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var categoryAttribute = new CategoryAttributeDTO
        {
            CategoryId = _categoryId,
            AttributeId = _attributeId,
            OrderPosition = 1
        };

        // Act
        _categoryAttributeRepository.Insert(categoryAttribute);

        var insertedCategoryAttributes = _categoryAttributeRepository
            .GetById(_categoryId)
            .ToList();

        // Assert
        Assert.That(insertedCategoryAttributes.Any(x =>
            x.CategoryId == _categoryId &&
            x.AttributeId == _attributeId &&
            x.OrderPosition == 1), Is.True);
    }


    [Test]
    public void InsertTest_ShouldNotInsertInvalidData()
    {
        // Arrange
        var categoryAttribute = new CategoryAttributeDTO
        {
            CategoryId = -9,
            AttributeId = _attributeId,
            OrderPosition = 1
        };

        // Act and Assert
        Assert.Throws<SqlException>(() =>
            _categoryAttributeRepository.Insert(categoryAttribute));
    }


    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var categoryAttribute = new CategoryAttributeDTO
        {
            CategoryId = _categoryId,
            AttributeId = _attributeId,
            OrderPosition = 1
        };

        _categoryAttributeRepository.Insert(categoryAttribute);

        categoryAttribute.OrderPosition = 5;

        // Act
        _categoryAttributeRepository.Update(categoryAttribute);

        var updatedCategoryAttributes = _categoryAttributeRepository
            .GetById(_categoryId)
            .ToList();

        // Assert
        Assert.That(updatedCategoryAttributes.Any(x =>
            x.CategoryId == _categoryId &&
            x.AttributeId == _attributeId &&
            x.OrderPosition == 5), Is.True);
    }


    [Test]
    public void UpdateTest_ShouldNotUpdateInvalidData()
    {
        // Arrange
        var categoryAttribute = new CategoryAttributeDTO
        {
            CategoryId = -9,
            AttributeId = -9,
            OrderPosition = 10
        };

        // Act and Assert
        Assert.Throws<SqlException>(() =>
            _categoryAttributeRepository.Update(categoryAttribute));
    }


    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var categoryAttribute = new CategoryAttributeDTO
        {
            CategoryId = _categoryId,
            AttributeId = _attributeId,
            OrderPosition = 1
        };

        _categoryAttributeRepository.Insert(categoryAttribute);

        // Act
        _categoryAttributeRepository.Delete(_categoryId, _attributeId);

        var deletedCategoryAttributes = _categoryAttributeRepository
            .GetById(_categoryId)
            .ToList();

        // Assert
        Assert.That(deletedCategoryAttributes.Any(x =>
            x.CategoryId == _categoryId &&
            x.AttributeId == _attributeId), Is.False);
    }


    [Test]
    public void DeleteTest_ShouldNotDeleteInvalidData()
    {
        // Act and Assert
        Assert.Throws<SqlException>(() =>
            _categoryAttributeRepository.Delete(-9, -9));
    }
}