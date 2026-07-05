// TODO: Delete. No need of separate tests for junction table

//using Market.DTO;
//using Market.Services.Interfaces.Repositories;
//using Market.Tests.Helpers;
//using Microsoft.Data.SqlClient;

//namespace Market.Tests;

//public class CategoryAttributeRepositoryTests : BaseRepositoryTests
//{
//    private SqlConnection _connection;
//    private ICategoryRepository _categoryRepository;
//    private AttributeRepository _attributeRepository;
//    private CategoryAttributeRepository _categoryAttributeRepository;
//    private int _categoryId;
//    private int _attributeId;

//    [SetUp]
//    public void Setup()
//    {
//        _connection = new SqlConnection(ConnectionString);
//        //UnitOfWork unitOfWork = new UnitOfWork(_connection);
//        //_categoryRepository = unitOfWork.CategoryRepository;
//        _attributeRepository = new AttributeRepository(_connection);
//        _categoryAttributeRepository = new CategoryAttributeRepository(_connection);

//        var category = new CategoryDTO
//        {
//            CategoryName = TestDataHelper.AddGuid("TestCategory"),
//            Description = "Test category description"
//        };
//        var attribute = new AttributeDTO
//        {
//            AttributeName = TestDataHelper.AddGuid("TestAttributeName"),
//            AttributeType = 1
//        };

//        _categoryId = _categoryRepository.Insert(category);
//        _attributeId = _attributeRepository.Insert(attribute);
//    }


//    [TearDown]
//    public void TearDown()
//    {
//        //_categoryRepository.Dispose();
//        _attributeRepository.Dispose();
//        _connection.Dispose();
//    }


//    [Test]
//    public void InsertTest_ShouldInsertValidData()
//    {
//        // Arrange
//        var categoryAttribute = new CategoryAttributeDTO
//        {
//            CategoryId = _categoryId,
//            AttributeId = _attributeId,
//            OrderPosition = 1
//        };

//        // Act
//        _categoryAttributeRepository.Insert(categoryAttribute);

//        var insertedCategoryAttributes = _categoryAttributeRepository
//            .GetById(_categoryId)
//            .ToList();

//        // Assert
//        Assert.That(insertedCategoryAttributes.Any(x =>
//            x.CategoryId == _categoryId &&
//            x.AttributeId == _attributeId &&
//            x.OrderPosition == 1), Is.True);
//    }


//    [Test]
//    public void InsertTest_ShouldNotInsertInvalidData()
//    {
//        // Arrange
//        var categoryAttribute = new CategoryAttributeDTO
//        {
//            CategoryId = -9,
//            AttributeId = _attributeId,
//            OrderPosition = 1
//        };

//        // Act and Assert
//        Assert.Throws<SqlException>(() =>
//            _categoryAttributeRepository.Insert(categoryAttribute));
//    }


//    [Test]
//    public void UpdateTest_ShouldUpdateValidData()
//    {
//        // Arrange
//        var categoryAttribute = new CategoryAttributeDTO
//        {
//            CategoryId = _categoryId,
//            AttributeId = _attributeId,
//            OrderPosition = 1
//        };

//        _categoryAttributeRepository.Insert(categoryAttribute);

//        categoryAttribute.OrderPosition = 5;

//        // Act
//        _categoryAttributeRepository.Update(categoryAttribute);

//        var updatedCategoryAttributes = _categoryAttributeRepository
//            .GetById(_categoryId)
//            .ToList();

//        // Assert
//        Assert.That(updatedCategoryAttributes.Any(x =>
//            x.CategoryId == _categoryId &&
//            x.AttributeId == _attributeId &&
//            x.OrderPosition == 5), Is.True);
//    }


//    [Test]
//    public void UpdateTest_ShouldNotUpdateInvalidData()
//    {
//        // Arrange
//        var categoryAttribute = new CategoryAttributeDTO
//        {
//            CategoryId = -9,
//            AttributeId = -9,
//            OrderPosition = 10
//        };

//        // Act and Assert
//        Assert.Throws<SqlException>(() =>
//            _categoryAttributeRepository.Update(categoryAttribute));
//    }


//    [Test]
//    public void DeleteTest_ShouldDeleteValidData()
//    {
//        // Arrange
//        var categoryAttribute = new CategoryAttributeDTO
//        {
//            CategoryId = _categoryId,
//            AttributeId = _attributeId,
//            OrderPosition = 1
//        };

//        _categoryAttributeRepository.Insert(categoryAttribute);

//        // Act
//        _categoryAttributeRepository.Delete(_categoryId, _attributeId);

//        var deletedCategoryAttributes = _categoryAttributeRepository
//            .GetById(_categoryId)
//            .ToList();

//        // Assert
//        Assert.That(deletedCategoryAttributes.Any(x =>
//            x.CategoryId == _categoryId &&
//            x.AttributeId == _attributeId), Is.False);
//    }


//    [Test]
//    public void DeleteTest_ShouldNotDeleteInvalidData()
//    {
//        // Act and Assert
//        Assert.Throws<SqlException>(() =>
//            _categoryAttributeRepository.Delete(-9, -9));
//    }

//    public override bool Equals(object? obj)
//    {
//        return obj is CategoryAttributeRepositoryTests tests &&
//               EqualityComparer<ICategoryRepository>.Default.Equals(_categoryRepository, tests._categoryRepository);
//    }
//}