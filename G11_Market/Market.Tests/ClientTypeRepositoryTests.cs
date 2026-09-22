using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Market.Tests;

[TestFixture]
public class ClientTypeRepositoryTests : BaseRepositoryTests
{
    private ClientTypeDTO CreateValidClientType() => new()
    {
        Name = "Type_" + Guid.NewGuid().ToString("N")[..10],
        Description = "Sample description for testing."
    };

    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var clientType = CreateValidClientType();

        // Act
        var newId = UnitOfWork.ClientTypeRepository.Insert(clientType);
        var inserted = UnitOfWork.ClientTypeRepository.GetById(newId);

        // Assert
        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(inserted, Is.Not.Null);
        Assert.That(inserted!.Id, Is.EqualTo(newId));
        Assert.That(inserted.Name, Is.EqualTo(clientType.Name));
        Assert.That(inserted.Description, Is.EqualTo(clientType.Description));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateName()
    {
        // Arrange
        var first = CreateValidClientType();
        var second = CreateValidClientType();
        second.Name = first.Name;

        UnitOfWork.ClientTypeRepository.Insert(first);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientTypeRepository.Insert(second));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyName()
    {
        // Arrange
        var clientType = CreateValidClientType();
        clientType.Name = "   ";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientTypeRepository.Insert(clientType));
    }

    [Test]
    public void Insert_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.ClientTypeRepository.Insert((ClientTypeDTO)null!));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var clientType = CreateValidClientType();
        var id = UnitOfWork.ClientTypeRepository.Insert(clientType);
        var inserted = UnitOfWork.ClientTypeRepository.GetById(id)!;

        inserted.Name = "Updated_" + Guid.NewGuid().ToString("N")[..10];
        inserted.Description = "Updated Description";

        // Act
        UnitOfWork.ClientTypeRepository.Update(inserted);
        var updated = UnitOfWork.ClientTypeRepository.GetById(id);

        // Assert
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Name, Is.EqualTo(inserted.Name));
        Assert.That(updated.Description, Is.EqualTo("Updated Description"));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateDuplicateName()
    {
        // Arrange
        var first = CreateValidClientType();
        var second = CreateValidClientType();

        var firstId = UnitOfWork.ClientTypeRepository.Insert(first);
        var secondId = UnitOfWork.ClientTypeRepository.Insert(second);

        var firstType = UnitOfWork.ClientTypeRepository.GetById(firstId)!;
        var secondType = UnitOfWork.ClientTypeRepository.GetById(secondId)!;

        secondType.Name = firstType.Name;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientTypeRepository.Update(secondType));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateNonExistentId()
    {
        // Arrange
        var nonExistent = CreateValidClientType();
        nonExistent.Id = -999;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientTypeRepository.Update(nonExistent));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.ClientTypeRepository.Update((ClientTypeDTO)null!));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var clientType = CreateValidClientType();
        var newId = UnitOfWork.ClientTypeRepository.Insert(clientType);

        // Act
        UnitOfWork.ClientTypeRepository.Delete(newId);
        var deleted = UnitOfWork.ClientTypeRepository.GetById(newId);

        // Assert
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public void DeleteTest_ShouldNotDeleteInvalidId()
    {
        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.ClientTypeRepository.Delete(-999));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.ClientTypeRepository.Delete((object)null!));
    }

    [Test]
    public void GetById_ShouldReturnCorrectClientType()
    {
        // Arrange
        var clientType = CreateValidClientType();
        var id = UnitOfWork.ClientTypeRepository.Insert(clientType);

        // Act
        var result = UnitOfWork.ClientTypeRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }

    [Test]
    public void GetById_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.ClientTypeRepository.GetById(-1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenDeleted_ShouldReturnNull()
    {
        // Arrange
        var clientType = CreateValidClientType();
        var id = UnitOfWork.ClientTypeRepository.Insert(clientType);
        UnitOfWork.ClientTypeRepository.Delete(id);

        // Act
        var result = UnitOfWork.ClientTypeRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.ClientTypeRepository.GetById((object)null!));
    }

    [Test]
    public void GetByName_ShouldReturnCorrectClientType()
    {
        // Arrange
        var clientType = CreateValidClientType();
        UnitOfWork.ClientTypeRepository.Insert(clientType);

        // Act
        var result = UnitOfWork.ClientTypeRepository.GetByName(clientType.Name);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo(clientType.Name));
    }

    [Test]
    public void GetAll_ShouldExcludeDeletedClientTypes()
    {
        // Arrange
        var active = CreateValidClientType();
        var deleted = CreateValidClientType();

        var activeId = UnitOfWork.ClientTypeRepository.Insert(active);
        var deletedId = UnitOfWork.ClientTypeRepository.Insert(deleted);

        UnitOfWork.ClientTypeRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.ClientTypeRepository.GetAll().ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == activeId), Is.True);
        Assert.That(results.Any(c => c.Id == deletedId), Is.False);
    }

    [Test]
    public void GetDeletedClientTypes_ShouldReturnOnlyDeletedClientTypes()
    {
        // Arrange
        var active = CreateValidClientType();
        var deleted = CreateValidClientType();

        var activeId = UnitOfWork.ClientTypeRepository.Insert(active);
        var deletedId = UnitOfWork.ClientTypeRepository.Insert(deleted);

        UnitOfWork.ClientTypeRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.ClientTypeRepository.GetDeletedClientTypes().ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == deletedId), Is.True);
        Assert.That(results.Any(c => c.Id == activeId), Is.False);
    }
}