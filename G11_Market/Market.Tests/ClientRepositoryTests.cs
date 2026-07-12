using Market.DTO;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class ClientRepositoryTests : BaseRepositoryTests
{
    [Test]

    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange

        var client = new ClientDTO
        {
            AccountId = 3,
            ClientTypeId = 1,
            FirstName = "Test",
            LastName = "Client",
            PhoneNumber = "555123456",
            ContactEmail = "test@gmail.com"
        };

        // Act

        var newId = UnitOfWork.ClientRepository.Insert(client);
        var inserted = UnitOfWork.ClientRepository.GetById(newId);

        // Assert

        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(inserted, Is.Not.Null);
        Assert.That(inserted!.FirstName, Is.EqualTo(client.FirstName));
        Assert.That(inserted.LastName, Is.EqualTo(client.LastName));
        Assert.That(inserted.AccountId, Is.EqualTo(client.AccountId));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidData()
    {
        // Arrange

        var client = new ClientDTO
        {
            AccountId = 3,
            ClientTypeId = 1,
            FirstName = "",
            LastName = "Test",
            PhoneNumber = "abc123",
            ContactEmail = "123"
        };

        // Act & Assert

        Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.Insert(client));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange

        var client = UnitOfWork.ClientRepository.GetById(UpdateTestId);

        client!.FirstName = "Updated";
        client.LastName = "Client";
        client.PhoneNumber = "555999888";
        client.ContactEmail = "updated@gmail.com";

        // Act

        UnitOfWork.ClientRepository.Update(client);
        var updated = UnitOfWork.ClientRepository.GetById(UpdateTestId);

        // Assert

        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.FirstName, Is.EqualTo("Updated"));
        Assert.That(updated.PhoneNumber, Is.EqualTo("555999888"));
        Assert.That(updated.ContactEmail, Is.EqualTo("updated@gmail.com"));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Act
        UnitOfWork.ClientRepository.Delete(DeleteTestId);

        // Assert
        var exception = Assert.Throws<SqlException>(() => UnitOfWork.ClientRepository.GetById(DeleteTestId));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Number, Is.EqualTo(50034));
    }
}