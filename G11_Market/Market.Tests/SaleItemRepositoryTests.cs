using Market.DTO;
//using Market.Repositories;
//using Market.Tests.Helpers;
//using Microsoft.Data.SqlClient;

//namespace Market.Tests;

//public class SaleItemRepositoryTests : BaseRepositoryTests
//{
//    private SqlConnection _connection;
//    private SaleItemRepository _repository;

//    [SetUp]
//    public void Setup()
//    {
//        DatabaseHelper.ClearDatabase();
//        DatabaseHelper.SeedDatabase();

//        _connection = new SqlConnection(ConnectionString);
//        _repository = new SaleItemRepository(_connection);
//    }

//    [TearDown]
//    public void TearDown()
//    {
//        _connection.Dispose();
//        _repository.Dispose();
//    }

//    [Test]
//    public void InsertTest_ShouldInsertValidData()
//    {
//        // Arrange

//        var saleItem = new SaleItemDTO
//        {
//            SaleId = 3,
//            ProductId = 3,
//            Quantity = 5,
//            UnitPrice = 300,
//            DiscountAmount = 0
//        };

//        // Act

//        var newId = _repository.Insert(saleItem);
//        var inserted = _repository.GetById(newId);

//        // Assert

//        Assert.That(newId, Is.GreaterThan(0));
//        Assert.That(inserted, Is.Not.Null);
//        Assert.That(inserted!.SaleId, Is.EqualTo(saleItem.SaleId));
//        Assert.That(inserted.ProductId, Is.EqualTo(saleItem.ProductId));
//        Assert.That(inserted.Quantity, Is.EqualTo(saleItem.Quantity));
//        Assert.That(inserted.UnitPrice, Is.EqualTo(saleItem.UnitPrice));
//    }

//    [Test]
//    public void InsertTest_ShouldNotInsertInvalidData()
//    {
//        // Arrange

//        var saleItem = new SaleItemDTO
//        {
//            SaleId = 0,
//            ProductId = 0,
//            Quantity = -1,
//            UnitPrice = -10.0m
//        };

//        // Act and Assert

//        Assert.Throws<SqlException>(() => _repository.Insert(saleItem));
//    }

//    [Test]

//    public void DeleteTest_ShouldRemoveItem()
//    {
//        // Arrange

//        var saleItem = new SaleItemDTO
//        {
//            SaleId = 3,
//            ProductId = 3,
//            Quantity = 5,
//            UnitPrice = 300,
//            DiscountAmount = 0
//        };

//        var newId = _repository.Insert(saleItem);

//        // Act

//        _repository.Delete(newId);

//        // Assert

//        Assert.Throws<InvalidOperationException>(() => _repository.GetById(newId));
//    }

//    [Test]
//    public void updateTest_ShouldModifyExistingItem()
//    {
//        // Arrange
//        var saleItem = _repository.GetById(UpdateTestId);

//        Assert.That(saleItem, Is.Not.Null);

//        saleItem!.Quantity = 10;
//        saleItem.UnitPrice = 150;
//        saleItem.DiscountAmount = 5;

//        // Act
//        _repository.Update(saleItem);

//        var updated = _repository.GetById(UpdateTestId);

//        // Assert
//        Assert.That(updated, Is.Not.Null);
//        Assert.That(updated!.Quantity, Is.EqualTo(10));
//        Assert.That(updated.UnitPrice, Is.EqualTo(150));
//    }
//}
////        using var connection = new SqlConnection(ConnectionString);
////        connection.Open();

////        using var command = new SqlCommand(SeedSaleItemDatabaseScript, connection);
////        command.ExecuteNonQuery();
////    }

////    [SetUp]
////    public void Setup()
////    {
////        ClearSaleItemDatabase();
////        SeedSaleItemDatabase();

////        _connection = new SqlConnection(ConnectionString);
////        _repository = new SaleItemRepository(_connection);
////    }

////    [TearDown]
////    public void TearDown()
////    {
////        _connection.Dispose();
////        _repository.Dispose();
////    }

////    [Test]
////    public void InsertTest_ShouldInsertValidData()
////    {
////        // Arrange

////        var saleItem = new SaleItemDTO
////        {
////            SaleId = 3,
////            ProductId = 3,
////            Quantity = 5,
////            UnitPrice = 300,
////            DiscountAmount = 0
////        };

////        // Act

////        var newId = _repository.Insert(saleItem);
////        var inserted = _repository.GetById(newId);

////        // Assert

////        Assert.That(newId, Is.GreaterThan(0));
////        Assert.That(inserted, Is.Not.Null);
////        Assert.That(inserted!.SaleId, Is.EqualTo(saleItem.SaleId));
////        Assert.That(inserted.ProductId, Is.EqualTo(saleItem.ProductId));
////        Assert.That(inserted.Quantity, Is.EqualTo(saleItem.Quantity));
////        Assert.That(inserted.UnitPrice, Is.EqualTo(saleItem.UnitPrice));
////    }

////    [Test]
////    public void InsertTest_ShouldNotInsertInvalidData()
////    {
////        // Arrange

////        var saleItem = new SaleItemDTO
////        {
////            SaleId = 0,
////            ProductId = 0,
////            Quantity = -1,
////            UnitPrice = -10.0m
////        };

////        // Act and Assert

////        Assert.Throws<SqlException>(() => _repository.Insert(saleItem));
////    }

////    [Test]

////    public void DeleteTest_ShouldRemoveItem()
////    {
////        // Arrange

////        var saleItem = new SaleItemDTO
////        {
////            SaleId = 3,
////            ProductId = 3,
////            Quantity = 5,
////            UnitPrice = 300,
////            DiscountAmount = 0
////        };

////        var newId = _repository.Insert(saleItem);

////        // Act

////        _repository.Delete(newId);

////        // Assert

////        Assert.Throws<InvalidOperationException>(() => _repository.GetById(newId));
////    }

////    [Test]
////    public void updateTest_ShouldModifyExistingItem()
////    {
////        // Arrange
////        var saleItem = _repository.GetById(UpdateTestId);

////        Assert.That(saleItem, Is.Not.Null);

////        saleItem!.Quantity = 10;
////        saleItem.UnitPrice = 150;
////        saleItem.DiscountAmount = 5;

////        // Act
////        _repository.Update(saleItem);

////        var updated = _repository.GetById(UpdateTestId);

////        // Assert
////        Assert.That(updated, Is.Not.Null);
////        Assert.That(updated!.Quantity, Is.EqualTo(10));
////        Assert.That(updated.UnitPrice, Is.EqualTo(150));
////    }
////}