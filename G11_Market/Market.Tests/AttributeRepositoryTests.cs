using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class AttributeRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var attribute = new AttributeDTO
        {
            AttributeName = "Color".AddGuid(),
            AttributeType = 1 // Text
        };

        // Act
        var newId = UnitOfWork.AttributeRepository.AssignAttribute(attribute);
        var insertedAttribute = UnitOfWork.AttributeRepository.GetById(newId);
        // Assert
        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(insertedAttribute, Is.Not.Null);
        Assert.That(insertedAttribute!.Id, Is.EqualTo(newId));
        Assert.That(insertedAttribute.AttributeName, Is.EqualTo(attribute.AttributeName));
        Assert.That(insertedAttribute.AttributeType, Is.EqualTo(attribute.AttributeType));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidAttributeType()
    {
        // Arrange
        var attribute = new AttributeDTO
        {
            AttributeName = "InvalidType".AddGuid(),
            AttributeType = 99
        };

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.AssignAttribute(attribute));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateAttributeName()
    {
        // Arrange
        var name = "DuplicateAttribute".AddGuid();

        var first = new AttributeDTO { AttributeName = name, AttributeType = 1 };
        var second = new AttributeDTO { AttributeName = name, AttributeType = 2 };

        UnitOfWork.AttributeRepository.AssignAttribute(first);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.AssignAttribute(second));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyAttributeName()
    {
        // Arrange
        var attribute = new AttributeDTO
        {
            AttributeName = "   ",
            AttributeType = 1
        };

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.AssignAttribute(attribute));
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

        var newId = UnitOfWork.AttributeRepository.AssignAttribute(attribute);
        var insertedAttribute = UnitOfWork.AttributeRepository.GetById(newId);

        insertedAttribute!.AttributeName = "UpdatedAttribute".AddGuid();
        insertedAttribute.AttributeType = 2;

        // Act
        UnitOfWork.AttributeRepository.Update(insertedAttribute);
        var updatedAttribute = UnitOfWork.AttributeRepository.GetById(newId);

        // Assert
        Assert.That(updatedAttribute, Is.Not.Null);
        Assert.That(updatedAttribute!.AttributeName, Is.EqualTo(insertedAttribute.AttributeName));
        Assert.That(updatedAttribute.AttributeType, Is.EqualTo(2));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateDuplicateAttributeName()
    {
        // Arrange
        var first = new AttributeDTO { AttributeName = "FirstAttr".AddGuid(), AttributeType = 1 };
        var second = new AttributeDTO { AttributeName = "SecondAttr".AddGuid(), AttributeType = 2 };

        var firstId = UnitOfWork.AttributeRepository.AssignAttribute(first);
        var secondId = UnitOfWork.AttributeRepository.AssignAttribute(second);

        var firstAttribute = UnitOfWork.AttributeRepository.GetById(firstId);
        var secondAttribute = UnitOfWork.AttributeRepository.GetById(secondId);

        secondAttribute!.AttributeName = firstAttribute!.AttributeName;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.Update(secondAttribute));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateInvalidAttributeType()
    {
        // Arrange
        var attribute = new AttributeDTO { AttributeName = "ValidType".AddGuid(), AttributeType = 1 };
        var id = UnitOfWork.AttributeRepository.AssignAttribute(attribute);
        var inserted = UnitOfWork.AttributeRepository.GetById(id)!;

        inserted.AttributeType = 99;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.Update(inserted));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateEmptyAttributeName()
    {
        // Arrange
        var attribute = new AttributeDTO { AttributeName = "ValidName".AddGuid(), AttributeType = 1 };
        var id = UnitOfWork.AttributeRepository.AssignAttribute(attribute);
        var inserted = UnitOfWork.AttributeRepository.GetById(id)!;

        inserted.AttributeName = "";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.Update(inserted));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateNonExistentId()
    {
        // Arrange
        var nonExistent = new AttributeDTO
        {
            Id = -999,
            AttributeName = "NonExistent".AddGuid(),
            AttributeType = 1
        };

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.Update(nonExistent));
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

        var newId = UnitOfWork.AttributeRepository.AssignAttribute(attribute);

        // Act
        UnitOfWork.AttributeRepository.Delete(newId);
        var deletedAttribute = UnitOfWork.AttributeRepository.GetById(newId);

        // Assert
        Assert.That(deletedAttribute, Is.Null);
    }

    [Test]
    public void DeleteTest_ShouldNotDeleteInvalidId()
    {
        // Arrange
        var invalidId = -999;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.AttributeRepository.Delete(invalidId));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.AttributeRepository.Delete(null!));
    }

    [Test]
    public void GetById_ShouldReturnCorrectAttribute()
    {
        // Arrange
        var attribute = new AttributeDTO
        {
            AttributeName = "GetByIdAttr".AddGuid(),
            AttributeType = 4 // Boolean
        };
        var insertedId = UnitOfWork.AttributeRepository.AssignAttribute(attribute);

        // Act
        var result = UnitOfWork.AttributeRepository.GetById(insertedId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(insertedId));
    }

    [Test]
    public void GetById_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.AttributeRepository.GetById(-1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.AttributeRepository.GetById((object)null!));
    }

    [Test]
    public void GetByName_ShouldReturnCorrectAttribute()
    {
        // Arrange
        var name = "FindByNameAttr".AddGuid();
        var attribute = new AttributeDTO
        {
            AttributeName = name,
            AttributeType = 1
        };
        UnitOfWork.AttributeRepository.AssignAttribute(attribute);

        // Act
        var result = UnitOfWork.AttributeRepository.GetByName(name);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.AttributeName, Is.EqualTo(name));
    }

    [Test]
    public void GetByName_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.AttributeRepository.GetByName("NonExistentAttribute".AddGuid());

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetByType_ShouldReturnMatchingAttributes()
    {
        // Arrange
        byte targetType = 3; 
        var attribute1 = new AttributeDTO { AttributeName = "Date1".AddGuid(), AttributeType = targetType };
        var attribute2 = new AttributeDTO { AttributeName = "Date2".AddGuid(), AttributeType = targetType };
        var attribute3 = new AttributeDTO { AttributeName = "Text1".AddGuid(), AttributeType = 1 };

        var id1 = UnitOfWork.AttributeRepository.AssignAttribute(attribute1);
        var id2 = UnitOfWork.AttributeRepository.AssignAttribute(attribute2);
        UnitOfWork.AttributeRepository.AssignAttribute(attribute3);

        // Act
        var results = UnitOfWork.AttributeRepository.GetByType(targetType).ToList();

        // Assert
        Assert.That(results, Is.Not.Null);
        Assert.That(results.Any(a => a.Id == id1), Is.True);
        Assert.That(results.Any(a => a.Id == id2), Is.True);
        Assert.That(results.All(a => a.AttributeType == targetType), Is.True);
    }

    [Test]
    public void GetAll_ShouldReturnAllInsertedRecords()
    {
        // Arrange
        var attribute1 = new AttributeDTO { AttributeName = "All1".AddGuid(), AttributeType = 1 };
        var attribute2 = new AttributeDTO { AttributeName = "All2".AddGuid(), AttributeType = 2 };

        UnitOfWork.AttributeRepository.AssignAttribute(attribute1);
        UnitOfWork.AttributeRepository.AssignAttribute(attribute2);

        // Act
        var allAttributes = UnitOfWork.AttributeRepository.GetAll().ToList();

        // Assert
        Assert.That(allAttributes.Count, Is.AtLeast(2));
    }

    [Test]
    public void GetById_WhenAttributeIsDeleted_ShouldReturnNull()
    {
        // Arrange
        var attribute = new AttributeDTO
        {
            AttributeName = "DeletedGetById".AddGuid(),
            AttributeType = 1
        };
        var id = UnitOfWork.AttributeRepository.AssignAttribute(attribute);

        // Soft-delete the attribute
        UnitOfWork.AttributeRepository.Delete(id);

        // Act
        var result = UnitOfWork.AttributeRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetAll_ShouldExcludeDeletedAttributes()
    {
        // Arrange
        var activeAttribute = new AttributeDTO { AttributeName = "Active".AddGuid(), AttributeType = 1 };
        var deletedAttribute = new AttributeDTO { AttributeName = "Deleted".AddGuid(), AttributeType = 1 };

        var activeId = UnitOfWork.AttributeRepository.AssignAttribute(activeAttribute);
        var deletedId = UnitOfWork.AttributeRepository.AssignAttribute(deletedAttribute);

        UnitOfWork.AttributeRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.AttributeRepository.GetAll().ToList();

        // Assert
        Assert.That(results.Any(a => a.Id == activeId), Is.True);
        Assert.That(results.Any(a => a.Id == deletedId), Is.False);
    }
}