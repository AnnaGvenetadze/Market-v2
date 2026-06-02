using Market.DTO;
using Market.Repositories;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class CountryRepositoryTests
{
    private const string ConnectionString = "Server=localhost;Database=MarketDB;Trusted_Connection=True;TrustServerCertificate=True;"; // gaasworet
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
        Assert.That(insertedCountry.CreateDate, Is.GreaterThan(DateTime.MinValue));
        Assert.That(insertedCountry.UpdateDate, Is.Null);
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidData()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = "საქართველო",
            CountryCode = "  " // "", null, საქ, s
        };

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Insert(country));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = "Testland",
            CountryCode = "TLT"
        };
        var newId = _repository.Insert(country);
        var insertedCountry = _repository.GetById(newId);
        insertedCountry!.Name = "Updatedland";
        insertedCountry.CountryCode = "UL";

        // Act
        _repository.Update(insertedCountry);
        var updatedCountry = _repository.GetById(newId);

        // Assert
        Assert.That(updatedCountry, Is.Not.Null);
        Assert.That(updatedCountry!.Name, Is.EqualTo("Updatedland"));
        Assert.That(updatedCountry.CountryCode, Is.EqualTo("UL"));
        Assert.That(updatedCountry.UpdateDate, Is.GreaterThan(DateTime.MinValue));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateInvalidData()
    {
        // Arrange
        var country = new CountryDTO
        {
            Name = "Testland",
            CountryCode = "LLT"
        };
        var newId = _repository.Insert(country);
        var insertedCountry = _repository.GetById(newId);
        insertedCountry!.Name = "Updatedland";
        insertedCountry.CountryCode = "s"; // "", "  ", null, საქ, s

        // Act and Assert
        Assert.Throws<SqlException>(() => _repository.Update(insertedCountry));
    }
}