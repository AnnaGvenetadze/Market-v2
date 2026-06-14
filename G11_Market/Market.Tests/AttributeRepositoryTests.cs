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
        public void InsertTest_ShouldInsertValidAttributeName()
        {
            // Arrange
            var attribute = new AttributeDTO
            {
                AttributeName = "Color".AddGuid(),
                AttributeType = 1
            };

            // Act
            var newId = _repository.Insert(attribute);

            // Assert
            Assert.That(newId, Is.GreaterThan(0));
        }

        [Test]
        public void InsertTest_ShouldNotInsertNullAttributeName()
        {
            // Arrange
            var attribute = new AttributeDTO
            {
                AttributeName = null!,
                AttributeType = 1
            };

            // Act and Assert
            Assert.Throws<SqlException>(() => _repository.Insert(attribute));
        }

        [Test]
        public void InsertTest_ShouldNotInsertBlankAttributeName()
        {
            // Arrange
            var attribute = new AttributeDTO
            {
                AttributeName = " ",
                AttributeType = 1
            };

            // Act and Assert
            Assert.Throws<SqlException>(() => _repository.Insert(attribute));
        }

        [Test]
        public void InsertTest_ShouldInsertValidAttributeType()
        {
            // Arrange
            var attribute = new AttributeDTO
            {
                AttributeName = "Size".AddGuid(),
                AttributeType = 2
            };

            // Act
            var newId = _repository.Insert(attribute);

            // Assert
            Assert.That(newId, Is.GreaterThan(0));
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

        [TearDown]
        public void TearDown()
        {
            _repository.Dispose();
            _connection.Dispose();
        }
    }
}