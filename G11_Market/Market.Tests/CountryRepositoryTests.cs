using Market.DTO;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class CountryRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = "TestLand",
            CountryCode = "TST"
        };

        // Act
        var newId = UnitOfWork.CountryRepository.AssignAttribute(country);
        var insertedCountry = UnitOfWork.CountryRepository.GetById(newId);

        // Assert
        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(insertedCountry, Is.Not.Null);
        Assert.That(insertedCountry!.Name, Is.EqualTo(country.Name));
        Assert.That(insertedCountry.CountryCode, Is.EqualTo(country.CountryCode));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateCountryCode()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = "DuplicateGeorgia",
            CountryCode = "GEO"
        };

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CountryRepository.AssignAttribute(country));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var existingCountry = UnitOfWork.CountryRepository.GetById(UpdateTestId);
        existingCountry!.Name = $"New{existingCountry.Name}";

        // Act
        UnitOfWork.CountryRepository.Update(existingCountry);
        var updatedCountry = UnitOfWork.CountryRepository.GetById(UpdateTestId);

        // Assert
        Assert.That(updatedCountry, Is.Not.Null);
        Assert.That(updatedCountry!.Name, Is.EqualTo(existingCountry.Name));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateDuplicateCountryCode()
    {
        // Arrange
        var countryToUpdate = UnitOfWork.CountryRepository.GetById(DeleteTestId);
        countryToUpdate!.CountryCode = "GEO";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CountryRepository.Update(countryToUpdate));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CountryRepository.Update(null!));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange, Act and Assert
        UnitOfWork.CountryRepository.Delete(DeleteTestId);
        Assert.Throws<SqlException>(() => UnitOfWork.CountryRepository.GetById(DeleteTestId));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CountryRepository.Delete(null!));
    }

    [Test]
    public void GetByCodeTest_ShouldReturnCountryByCode()
    {
        // Act
        var country = UnitOfWork.CountryRepository.GetByCode("GEO");

        // Assert
        Assert.That(country, Is.Not.Null);
        Assert.That(country!.CountryCode, Is.EqualTo("GEO"));
        Assert.That(country.Name, Is.EqualTo("Georgia"));
    }

    [Test]
    public void GetByCodeTest_ShouldReturnNullWhenCodeDoesNotExist()
    {
        // Act
        var country = UnitOfWork.CountryRepository.GetByCode("ZZZ");

        // Assert
        Assert.That(country, Is.Null);
    }
}