using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Market.Tests;

[TestFixture]
public class CountryRepositoryTests : BaseRepositoryTests
{
    private static CountryDTO CreateValidCountry() => new()
    {
        Name = "Country_" + Guid.NewGuid().ToString("N")[..8],
        CountryCode = "C" + Random.Shared.Next(10, 99).ToString()
    };

    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var country = CreateValidCountry();

        // Act
        var insertedId = UnitOfWork.CountryRepository.Insert(country);
        var inserted = UnitOfWork.CountryRepository.GetById(insertedId);

        // Assert
        Assert.That(insertedId, Is.GreaterThan(0));
        Assert.That(inserted, Is.Not.Null);
        Assert.That(inserted!.Id, Is.EqualTo(insertedId));
        Assert.That(inserted.Name, Is.EqualTo(country.Name));
        Assert.That(inserted.CountryCode, Is.EqualTo(country.CountryCode));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateCountryCode()
    {
        // Arrange
        var first = CreateValidCountry();
        var second = CreateValidCountry();
        second.CountryCode = first.CountryCode;

        UnitOfWork.CountryRepository.Insert(first);

        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CountryRepository.Insert(second));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyName()
    {
        // Arrange
        var country = CreateValidCountry();
        country.Name = "   ";

        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CountryRepository.Insert(country));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidCountryCode()
    {
        // Arrange
        var country = CreateValidCountry();
        country.CountryCode = "A"; // Less than 2 characters

        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CountryRepository.Insert(country));
    }

    [Test]
    public void Insert_WhenEntityIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CountryRepository.Insert((CountryDTO)null!));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var country = CreateValidCountry();
        var id = UnitOfWork.CountryRepository.Insert(country);
        var inserted = UnitOfWork.CountryRepository.GetById(id)!;

        inserted.Name = "Updated Country Name";
        inserted.CountryCode = "UPD";

        // Act
        UnitOfWork.CountryRepository.Update(inserted);
        var updated = UnitOfWork.CountryRepository.GetById(id);

        // Assert
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Name, Is.EqualTo("Updated Country Name"));
        Assert.That(updated.CountryCode, Is.EqualTo("UPD"));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateDuplicateCountryCode()
    {
        // Arrange
        var first = CreateValidCountry();
        var second = CreateValidCountry();

        var firstId = UnitOfWork.CountryRepository.Insert(first);
        var secondId = UnitOfWork.CountryRepository.Insert(second);

        var secondCountry = UnitOfWork.CountryRepository.GetById(secondId)!;
        secondCountry.CountryCode = first.CountryCode;

        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CountryRepository.Update(secondCountry));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateNonExistentId()
    {
        // Arrange
        var nonExistent = CreateValidCountry();
        nonExistent.Id = -999;

        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CountryRepository.Update(nonExistent));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CountryRepository.Update((CountryDTO)null!));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var country = CreateValidCountry();
        var id = UnitOfWork.CountryRepository.Insert(country);

        // Act
        UnitOfWork.CountryRepository.Delete(id);
        var deleted = UnitOfWork.CountryRepository.GetById(id);

        // Assert
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public void DeleteTest_ShouldNotDeleteInvalidId()
    {
        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CountryRepository.Delete(-999));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CountryRepository.Delete((object)null!));
    }

    [Test]
    public void GetById_ShouldReturnCorrectCountry()
    {
        // Arrange
        var country = CreateValidCountry();
        var id = UnitOfWork.CountryRepository.Insert(country);

        // Act
        var result = UnitOfWork.CountryRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }

    [Test]
    public void GetById_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.CountryRepository.GetById(-1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetByCode_ShouldReturnCorrectCountry()
    {
        // Arrange
        var country = CreateValidCountry();
        UnitOfWork.CountryRepository.Insert(country);

        // Act
        var result = UnitOfWork.CountryRepository.GetByCode(country.CountryCode);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CountryCode, Is.EqualTo(country.CountryCode));
    }

    [Test]
    public void GetAll_ShouldExcludeDeletedCountries()
    {
        // Arrange
        var active = CreateValidCountry();
        var deleted = CreateValidCountry();

        var activeId = UnitOfWork.CountryRepository.Insert(active);
        var deletedId = UnitOfWork.CountryRepository.Insert(deleted);

        UnitOfWork.CountryRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.CountryRepository.GetAll().ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == activeId), Is.True);
        Assert.That(results.Any(c => c.Id == deletedId), Is.False);
    }
}