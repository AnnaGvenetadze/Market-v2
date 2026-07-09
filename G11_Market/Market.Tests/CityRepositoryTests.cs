using Market.DTO;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class CityRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var city = new CityDTO
        {
            Name = "TestCity",
            CountryId = UpdateTestId
        };

        // Act
        var newId = UnitOfWork.CityRepository.Insert(city);
        var insertedCity = UnitOfWork.CityRepository.GetById(newId);

        // Assert
        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(insertedCity, Is.Not.Null);
        Assert.That(insertedCity!.Name, Is.EqualTo(city.Name));
        Assert.That(insertedCity.CountryId, Is.EqualTo(city.CountryId));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidCountryId()
    {
        // Arrange
        var city = new CityDTO
        {
            Name = "WrongCity",
            CountryId = -1
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.CityRepository.Insert(city));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var existingCity = UnitOfWork.CityRepository.GetById(UpdateTestId);
        existingCity!.Name = "UpdatedCity";

        // Act
        UnitOfWork.CityRepository.Update(existingCity);

        var updatedCity =
            UnitOfWork.CityRepository.GetById(UpdateTestId);

        // Assert
        Assert.That(updatedCity, Is.Not.Null);
        Assert.That(updatedCity!.Name, Is.EqualTo("UpdatedCity"));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.CityRepository.Update(null!));
    }
    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange, Act
        UnitOfWork.CityRepository.Delete(DeleteTestId);

        // Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.CityRepository.GetById(DeleteTestId));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.CityRepository.Delete(null!));
    }

    [Test]
    public void GetByNameTest_ShouldReturnCityByName()
    {
        // Arrange
        var city = new CityDTO
        {
            Name = "Rustavi",
            CountryId = UpdateTestId
        };

        var newId = UnitOfWork.CityRepository.Insert(city);

        // Act
        var foundCity = UnitOfWork.CityRepository.GetByName(city.Name);

        // Assert
        Assert.That(foundCity, Is.Not.Null);
        Assert.That(foundCity!.Id, Is.EqualTo(newId));
        Assert.That(foundCity.Name, Is.EqualTo(city.Name));
    }

    [Test]
    public void GetByNameTest_ShouldReturnNullWhenNameDoesNotExist()
    {
        // Act
        var city = UnitOfWork.CityRepository.GetByName("CityThatDoesNotExist");

        // Assert
        Assert.That(city, Is.Null);
    }

    [Test]
    public void GetAllActive_ShouldReturnOnlyActiveCities()
    {
        // Arrange
        var city = new CityDTO
        {
            Name = "ActiveCity",
            CountryId = UpdateTestId
        };

        var newId = UnitOfWork.CityRepository.Insert(city);

        // Act
        var cities = UnitOfWork.CityRepository.GetAllActive().ToList();

        // Assert
        Assert.That(cities.Any(x => x.Id == newId), Is.True);
        Assert.That(cities.All(x => x.IsDeleted == false), Is.True);
    }

    [Test]
    public void GetAll_ShouldReturnCities()
    {
        // Act
        var cities = UnitOfWork.CityRepository.GetAll().ToList();

        // Assert
        Assert.That(cities.Count, Is.GreaterThan(0));
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.CityRepository.GetById(null!));
    }
}