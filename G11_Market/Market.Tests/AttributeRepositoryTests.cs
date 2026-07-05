using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests
{
    public class AttributeRepositoryTests : BaseRepositoryTests
    {
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
            var newId = UnitOfWork.AttributeRepository.Insert(attribute);
            var insertedAttribute = UnitOfWork.AttributeRepository.GetById(newId);

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
            Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.Insert(attribute));
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
            UnitOfWork.AttributeRepository.Insert(first);

            // Assert
            Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.Insert(second));
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

            var newId = UnitOfWork.AttributeRepository.Insert(attribute);
            var insertedAttribute = UnitOfWork.AttributeRepository.GetById(newId);

            insertedAttribute!.AttributeName = "UpdatedAttribute".AddGuid();

            // Act
            UnitOfWork.AttributeRepository.Update(insertedAttribute);

            var updatedAttribute = UnitOfWork.AttributeRepository.GetById(newId);

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

            var firstId = UnitOfWork.AttributeRepository.Insert(first);
            var secondId = UnitOfWork.AttributeRepository.Insert(second);

            var firstAttribute = UnitOfWork.AttributeRepository.GetById(firstId);
            var secondAttribute = UnitOfWork.AttributeRepository.GetById(secondId);

            secondAttribute!.AttributeName = firstAttribute!.AttributeName;

            // Act and Assert
            Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.Update(secondAttribute));
        }

        [Test]
        public void Update_WhenEntityIsNull_ShouldThrowException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => UnitOfWork.AttributeRepository.Update(null!));
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

            var newId = UnitOfWork.AttributeRepository.Insert(attribute);

            // Act
            UnitOfWork.AttributeRepository.Delete(newId);

            // Assert
            Assert.Throws<InvalidOperationException>(() => UnitOfWork.AttributeRepository.GetById(newId));
        }

        [Test]
        public void Delete_WhenIdIsNull_ShouldThrowException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => UnitOfWork.AttributeRepository.Delete(null!));
        }

        [Test]
        public void DeleteTest_ShouldNotDeleteInvalidId()
        {
            // Arrange
            var invalidId = -9;

            // Act and Assert
            Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.Delete(invalidId));
        }
    }
}