using Market.DTO;
using Market.Repositories;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class CountryRepositoryTests : BaseRepositoryTests
{
    private SqlConnection _connection;
    private CountryRepository _repository;

    [SetUp]
    public void Setup()
    {
        _connection = new SqlConnection(ConnectionString);
        _repository = new CountryRepository(_connection);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Dispose();
        _connection.Dispose();
    }

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
        var newId = _repository.Insert(country);
        var insertedCountry = _repository.GetById(newId);

        // Assert
        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(insertedCountry, Is.Not.Null);
        Assert.That(insertedCountry!.Name, Is.EqualTo(country.Name));
        Assert.That(insertedCountry.CountryCode, Is.EqualTo(country.CountryCode));
    }

    [Test]
    public void InsertTest_ShouldNotInsertNameWithTrailingSpaces()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = " TestCountry ",
            CountryCode = "NSA"
        };

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Insert(country));
    }

    [Test]
    public void InsertTest_ShouldNotInsertCountryCodeWithTrailingSpaces()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = "SpacedCodeCountry",
            CountryCode = " AB"
        };

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Insert(country));
    }

    [Test]
    public void InsertTest_ShouldNotInsertCountryCodeShorterThanTwoCharacters()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = "ShortCodeCountry",
            CountryCode = "A"
        };

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Insert(country));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateCountryCode()
    {
        // Arrange
        var first = new CountryDTO
        {
            Name = "FirstCountry",
            CountryCode = "DUP"
        };

        var second = new CountryDTO
        {
            Name = "SecondCountry",
            CountryCode = "DUP"
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
        var existingCountry = _repository.GetById(UpdateTestId);
        existingCountry.Name = $"New{existingCountry.Name}";

        // Act
        _repository.Update(existingCountry);
        var updatedCountry = _repository.GetById(UpdateTestId);

        // Assert
        Assert.That(updatedCountry, Is.Not.Null);
        Assert.That(updatedCountry!.Name, Is.EqualTo(existingCountry.Name));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateBlankName()
    {
        // Arrange
        var existingCountry = _repository.GetById(UpdateTestId);
        existingCountry.Name = " ";

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Update(existingCountry));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateNameTrailingSpaces()
    {
        // Arrange
        var existingCountry = _repository.GetById(UpdateTestId);
        existingCountry.Name = " UpdatedCountry ";

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Update(existingCountry));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateBlankCountryCode()
    {
        // Arrange
        var existingCountry = _repository.GetById(UpdateTestId);
        existingCountry.CountryCode = " ";

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Update(existingCountry));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateCountryCodeShorterThanTwoCharacters()
    {
        // Arrange
        var existingCountry = _repository.GetById(UpdateTestId);
        existingCountry.CountryCode = "A";

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Update(existingCountry));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateCountryCodeWithTrailingSpaces()
    {
        // Arrange
        var existingCountry = _repository.GetById(UpdateTestId);
        existingCountry.CountryCode = " AB";

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Update(existingCountry));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateDuplicateCountryCode()
    {
        // Arrange
        var first = new CountryDTO
        {
            Name = "FirstUpdateCountry",
            CountryCode = "UPA"
        };

        var second = new CountryDTO
        {
            Name = "SecondUpdateCountry",
            CountryCode = "UPB"
        };

        _repository.Insert(first);
        var secondId = _repository.Insert(second);

        var countryToUpdate = _repository.GetById(secondId);
        countryToUpdate.CountryCode = first.CountryCode;

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Update(countryToUpdate));
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
        // Arrange, Act and Assert
        _repository.Delete(DeleteTestId);
        Assert.Throws<SqlException>(() => _repository.GetById(DeleteTestId));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Delete(null!));
    }

    [Test]
    public void GetByCodeTest_ShouldReturnCountryByCode()
    {
        // Act
        var country = _repository.GetByCode("GEO");

        // Assert
        Assert.That(country, Is.Not.Null);
        Assert.That(country!.CountryCode, Is.EqualTo("GEO"));
        Assert.That(country.Name, Is.EqualTo("Georgia"));
    }

    [Test]
    public void GetByCodeTest_ShouldReturnNullWhenCodeDoesNotExist()
    {
        // Act
        var country = _repository.GetByCode("ZZZ");

        // Assert
        Assert.That(country, Is.Null);
    }

    [Test]
    public void GetByCodeTest_WhenCodeIsBlank_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentException>(() => _repository.GetByCode(" "));
    }
}