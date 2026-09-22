using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Market.Tests;

[TestFixture]
public class CityRepositoryTests : BaseRepositoryTests
{
    private int _testCountryId;

    [SetUp]
    public void SetUpCountry()
    {
        var country = new CountryDTO
        {
            Name = "Country_".AddGuid(),
            CountryCode = Guid.NewGuid().ToString("N")[..3].ToUpper()
        };
        _testCountryId = UnitOfWork.CountryRepository.Insert(country);
    }

    private CityDTO CreateValidCity(int? countryId = null) => new()
    {
        Name = "City_".AddGuid(),
        CountryId = countryId ?? _testCountryId
    };

    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var city = CreateValidCity();

        // Act
        var newId = UnitOfWork.CityRepository.Insert(city);
        var insertedCity = UnitOfWork.CityRepository.GetById(newId);

        // Assert
        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(insertedCity, Is.Not.Null);
        Assert.That(insertedCity!.Id, Is.EqualTo(newId));
        Assert.That(insertedCity.Name, Is.EqualTo(city.Name));
        Assert.That(insertedCity.CountryId, Is.EqualTo(city.CountryId));
    }

    [Test]
    public void InsertTest_InvalidCountryId_ShouldThrowSqlException()
    {
        // Arrange
        var city = CreateValidCity(countryId: -999);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CityRepository.Insert(city));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyName()
    {
        // Arrange
        var city = CreateValidCity();
        city.Name = "   ";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CityRepository.Insert(city));
    }

    [Test]
    public void Insert_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CityRepository.Insert((CityDTO)null!));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var city = CreateValidCity();
        var id = UnitOfWork.CityRepository.Insert(city);
        var insertedCity = UnitOfWork.CityRepository.GetById(id)!;

        insertedCity.Name = "UpdatedCity_".AddGuid();

        // Act
        UnitOfWork.CityRepository.Update(insertedCity);
        var updatedCity = UnitOfWork.CityRepository.GetById(id);

        // Assert
        Assert.That(updatedCity, Is.Not.Null);
        Assert.That(updatedCity!.Name, Is.EqualTo(insertedCity.Name));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateInvalidCountryId()
    {
        // Arrange
        var city = CreateValidCity();
        var id = UnitOfWork.CityRepository.Insert(city);
        var insertedCity = UnitOfWork.CityRepository.GetById(id)!;

        insertedCity.CountryId = -999;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CityRepository.Update(insertedCity));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateEmptyName()
    {
        // Arrange
        var city = CreateValidCity();
        var id = UnitOfWork.CityRepository.Insert(city);
        var insertedCity = UnitOfWork.CityRepository.GetById(id)!;

        insertedCity.Name = "   ";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CityRepository.Update(insertedCity));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateNonExistentId()
    {
        // Arrange
        var nonExistent = CreateValidCity();
        nonExistent.Id = -999;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CityRepository.Update(nonExistent));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CityRepository.Update((CityDTO)null!));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var city = CreateValidCity();
        var newId = UnitOfWork.CityRepository.Insert(city);

        // Act
        UnitOfWork.CityRepository.Delete(newId);
        var deletedCity = UnitOfWork.CityRepository.GetById(newId);

        // Assert
        Assert.That(deletedCity, Is.Null);
    }

    [Test]
    public void DeleteTest_ShouldNotDeleteInvalidId()
    {
        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CityRepository.Delete(-999));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CityRepository.Delete((object)null!));
    }

    [Test]
    public void GetById_ShouldReturnCorrectCity()
    {
        // Arrange
        var city = CreateValidCity();
        var id = UnitOfWork.CityRepository.Insert(city);

        // Act
        var result = UnitOfWork.CityRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }

    [Test]
    public void GetById_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.CityRepository.GetById(-1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenDeleted_ShouldReturnNull()
    {
        // Arrange
        var city = CreateValidCity();
        var id = UnitOfWork.CityRepository.Insert(city);
        UnitOfWork.CityRepository.Delete(id);

        // Act
        var result = UnitOfWork.CityRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CityRepository.GetById((object)null!));
    }

    [Test]
    public void GetByNameAndCountryId_ShouldReturnCorrectCity()
    {
        // Arrange
        var city = CreateValidCity();
        UnitOfWork.CityRepository.Insert(city);

        // Act
        var result = UnitOfWork.CityRepository.GetByNameAndCountryId(city.Name, city.CountryId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo(city.Name));
        Assert.That(result.CountryId, Is.EqualTo(city.CountryId));
    }

    [Test]
    public void GetByNameAndCountryId_WithLeadingTrailingWhitespace_ShouldTrimAndReturnCorrectCity()
    {
        // Arrange
        var city = CreateValidCity();
        UnitOfWork.CityRepository.Insert(city);

        // Act
        var result = UnitOfWork.CityRepository.GetByNameAndCountryId($"  {city.Name}  ", city.CountryId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo(city.Name));
    }

    [Test]
    public void GetByNameAndCountryId_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.CityRepository.GetByNameAndCountryId("NonExistentCity_".AddGuid(), _testCountryId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetByNameAndCountryId_InvalidInputs_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CityRepository.GetByNameAndCountryId(null!, _testCountryId));
        Assert.Throws<ArgumentException>(() => UnitOfWork.CityRepository.GetByNameAndCountryId("  ", _testCountryId));
        Assert.Throws<ArgumentOutOfRangeException>(() => UnitOfWork.CityRepository.GetByNameAndCountryId("ValidName", 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => UnitOfWork.CityRepository.GetByNameAndCountryId("ValidName", -1));
    }

    [Test]
    public void GetByCountryId_ShouldReturnMatchingCities()
    {
        // Arrange
        var first = CreateValidCity();
        var second = CreateValidCity();

        var id1 = UnitOfWork.CityRepository.Insert(first);
        var id2 = UnitOfWork.CityRepository.Insert(second);

        // Act
        var results = UnitOfWork.CityRepository.GetByCountryId(_testCountryId).ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == id1), Is.True);
        Assert.That(results.Any(c => c.Id == id2), Is.True);
        Assert.That(results.All(c => c.CountryId == _testCountryId), Is.True);
    }

    [Test]
    public void GetByCountryId_InvalidCountryId_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => UnitOfWork.CityRepository.GetByCountryId(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => UnitOfWork.CityRepository.GetByCountryId(-1));
    }

    [Test]
    public void GetAll_ShouldExcludeDeletedCities()
    {
        // Arrange
        var active = CreateValidCity();
        var deleted = CreateValidCity();

        var activeId = UnitOfWork.CityRepository.Insert(active);
        var deletedId = UnitOfWork.CityRepository.Insert(deleted);

        UnitOfWork.CityRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.CityRepository.GetAll().ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == activeId), Is.True);
        Assert.That(results.Any(c => c.Id == deletedId), Is.False);
    }

    [Test]
    public void GetDeletedCities_ShouldReturnOnlyDeletedCities()
    {
        // Arrange
        var active = CreateValidCity();
        var deleted = CreateValidCity();

        var activeId = UnitOfWork.CityRepository.Insert(active);
        var deletedId = UnitOfWork.CityRepository.Insert(deleted);

        UnitOfWork.CityRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.CityRepository.GetDeletedCities().ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == deletedId), Is.True);
        Assert.That(results.Any(c => c.Id == activeId), Is.False);
    }
}