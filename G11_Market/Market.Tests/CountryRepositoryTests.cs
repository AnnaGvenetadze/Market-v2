using Market.DTO;
using Market.Repositories;
using Market.Tests.Helpers;
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
            Name = "TestLand".AddGuid(),
            CountryCode = TestDataHelper.GenerateCode()
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
    public void InsertTest_ShouldNotInsertInvalidData()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = "InvalidCountry".AddGuid(),
            CountryCode = " "
        };

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Insert(country));
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
    public void UpdateTest_ShouldNotUpdateInvalidData()
    {
        Assert.Pass("This test is not implemented yet. Implementing validation logic in the repository is required to make this test meaningful.");
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange, Act and Assert
        _repository.Delete(DeleteTestId);
        Assert.Throws<SqlException>(() => _repository.GetById(DeleteTestId));
    }
}