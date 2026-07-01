using Market.DTO;
using Market.Repositories;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests
{
    public class AttributeRepositoryTests : BaseRepositoryTests
    {
        private SqlConnection _connection;
        private AttributeRepository _repository;

        [SetUp]
        public void Setup()
        {
            _connection = new SqlConnection(ConnectionString);
            _repository = new AttributeRepository(_connection);
        }

        [Test]
        public void InsertTest_ShouldInsertValidData()
        {
            // Arrange
            var attribute = new AttributeDTO
            {
                AttributeName = "Color".AddGuid(),
                AttributeType = 1
            };

            // Act
            var newId = _repository.Insert(attribute);
            var insertedAttribute = _repository.GetById(newId);

            // Assert
            Assert.That(newId, Is.GreaterThan(0));
            Assert.That(insertedAttribute, Is.Not.Null);
            Assert.That(insertedAttribute!.AttributeName, Is.EqualTo(attribute.AttributeName));
            Assert.That(insertedAttribute!.AttributeType, Is.EqualTo(attribute.AttributeType));
        }

        [Test]
        public void InsertTest_ShouldNotInsertInvalidAttributeType()
        {
            // Arrange
            var attribute = new AttributeDTO
            {
                AttributeName = "InvalidType".AddGuid(),
                AttributeType = 9
            };

            // Act and Assert
            Assert.Throws<SqlException>(() => _repository.Insert(attribute));
        }

        [Test]
        public void InsertTest_ShouldNotInsertDuplicateAttributeName()
        {
            // Arrange
            var name = "DuplicateAttribute".AddGuid();

            var first = new AttributeDTO
            {
                AttributeName = name,
                AttributeType = 1
            };

            var second = new AttributeDTO
            {
                AttributeName = name,
                AttributeType = 2
            };

            // Act
            _repository.Insert(first);

            // Assert
            Assert.Throws<SqlException>(() => _repository.Insert(second));
        }

        [Test]
        public void UpdateTest_ShouldUpdateValidData()
        {
            // Arrange
            var attribute = new AttributeDTO
            {
                AttributeName = "OldAttribute".AddGuid(),
                AttributeType = 1
            };

            var newId = _repository.Insert(attribute);
            var insertedAttribute = _repository.GetById(newId);

            insertedAttribute!.AttributeName = "UpdatedAttribute".AddGuid();

            // Act
            _repository.Update(insertedAttribute);

            var updatedAttribute = _repository.GetById(newId);

            // Assert
            Assert.That(updatedAttribute, Is.Not.Null);
            Assert.That(updatedAttribute!.AttributeName, Is.EqualTo(insertedAttribute.AttributeName));
            Assert.That(updatedAttribute!.AttributeType, Is.EqualTo(insertedAttribute.AttributeType));
        }

        [Test]
        public void UpdateTest_ShouldNotUpdateDuplicateAttributeName()
        {
            // Arrange
            var first = new AttributeDTO
            {
                AttributeName = "FirstAttribute".AddGuid(),
                AttributeType = 1
            };

            var second = new AttributeDTO
            {
                AttributeName = "SecondAttribute".AddGuid(),
                AttributeType = 2
            };

            var firstId = _repository.Insert(first);
            var secondId = _repository.Insert(second);

            var firstAttribute = _repository.GetById(firstId);
            var secondAttribute = _repository.GetById(secondId);

            secondAttribute!.AttributeName = firstAttribute!.AttributeName;

            // Act and Assert
            Assert.Throws<SqlException>(() => _repository.Update(secondAttribute));
        }

        [Test]
        public void Update_WhenEntityIsNull_ShouldThrowException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => _repository.Update(null!));
        }

        [Test]
        public void DeleteTest_ShouldDeleteValidData()
        {
            // Arrange
            var attribute = new AttributeDTO
            {
                AttributeName = "DeleteAttribute".AddGuid(),
                AttributeType = 1
            };

            var newId = _repository.Insert(attribute);

            // Act
            _repository.Delete(newId);

            // Assert
            Assert.Throws<InvalidOperationException>(() => _repository.GetById(newId));
        }

        [Test]
        public void Delete_WhenIdIsNull_ShouldThrowException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => _repository.Delete(null!));
        }

        [Test]
        public void DeleteTest_ShouldNotDeleteInvalidId()
        {
            // Arrange
            var invalidId = -9;

            // Act and Assert
            Assert.Throws<SqlException>(() => _repository.Delete(invalidId));
        }

        [TearDown]
        public void TearDown()
        {
            _repository.Dispose();
            _connection.Dispose();
        }
    }
}