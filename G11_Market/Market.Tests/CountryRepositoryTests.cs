using Market.DTO;
using Market.Repositories;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class CountryRepositoryTests
{
    private const string ConnectionString = "Your"; // gaasworet
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
        _connection.Dispose();
    }

    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = "Testland",
            CountryCode = "TL"
        };

        // Act
        var newId = _repository.Insert(country);
        var insertedCountry = _repository.GetById(newId);

        // Assert
        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(insertedCountry, Is.Not.Null);
        Assert.That(insertedCountry!.Name, Is.EqualTo(country.Name));
        Assert.That(insertedCountry!.CountryCode, Is.EqualTo(country.CountryCode));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidData()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = null,
            CountryCode = null
        };

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Insert(country));
    }
}