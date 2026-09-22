using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Market.Tests;

[TestFixture]
public class CorporateClientDetailsRepositoryTests : BaseRepositoryTests
{
    private int CreateTestAccount()
    {
        var account = new AccountDTO
        {
            Username = "User_" + Guid.NewGuid().ToString("N")[..10],
            PasswordHash = "HashedPassword123!",
            Email = $"user_{Guid.NewGuid()}@test.com",
            AccountType = 1,
            FirstName = "John",
            LastName = "Doe"
        };

        var accountId = UnitOfWork.AccountRepository.Insert(account);
        Assert.That(accountId, Is.GreaterThan(0), $"AccountRepository.Insert returned {accountId}");

        return accountId;
    }

    private CorporateClientDetailsDTO CreateValidCorporateClientDetails(int? id = null) => new()
    {
        Id = CreateTestAccount(),
        CompanyName = "Company_" + Guid.NewGuid().ToString("N")[..8],
        TaxNumber = "TAX" + Random.Shared.NextInt64(1000000000, 9999999999),
        LegalAddress = "123 Business Way, Suite 100",
        ContactPersonName = "John Doe"
    };

    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var clientDetails = CreateValidCorporateClientDetails();

        // Act
        var insertedId = UnitOfWork.CorporateClientDetailsRepository.Insert(clientDetails);
        var inserted = UnitOfWork.CorporateClientDetailsRepository.GetById(insertedId);

        // Assert
        Assert.That(insertedId, Is.EqualTo(clientDetails.Id));
        Assert.That(inserted, Is.Not.Null);
        Assert.That(inserted!.Id, Is.EqualTo(clientDetails.Id));
        Assert.That(inserted.CompanyName, Is.EqualTo(clientDetails.CompanyName));
        Assert.That(inserted.TaxNumber, Is.EqualTo(clientDetails.TaxNumber));
        Assert.That(inserted.LegalAddress, Is.EqualTo(clientDetails.LegalAddress));
        Assert.That(inserted.ContactPersonName, Is.EqualTo(clientDetails.ContactPersonName));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateTaxNumber()
    {
        // Arrange
        var first = CreateValidCorporateClientDetails();
        var second = CreateValidCorporateClientDetails();
        second.TaxNumber = first.TaxNumber;

        UnitOfWork.CorporateClientDetailsRepository.Insert(first);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CorporateClientDetailsRepository.Insert(second));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidAccountId()
    {
        // Arrange
        var clientDetails = CreateValidCorporateClientDetails(id: -999);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CorporateClientDetailsRepository.Insert(clientDetails));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyCompanyName()
    {
        // Arrange
        var clientDetails = CreateValidCorporateClientDetails();
        clientDetails.CompanyName = "   ";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CorporateClientDetailsRepository.Insert(clientDetails));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyTaxNumber()
    {
        // Arrange
        var clientDetails = CreateValidCorporateClientDetails();
        clientDetails.TaxNumber = "   ";

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CorporateClientDetailsRepository.Insert(clientDetails));
    }

    [Test]
    public void Insert_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CorporateClientDetailsRepository.Insert((CorporateClientDetailsDTO)null!));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var clientDetails = CreateValidCorporateClientDetails();
        var id = UnitOfWork.CorporateClientDetailsRepository.Insert(clientDetails);
        var inserted = UnitOfWork.CorporateClientDetailsRepository.GetById(id)!;

        inserted.CompanyName = "Updated Company Ltd";
        inserted.ContactPersonName = "Jane Smith";

        // Act
        UnitOfWork.CorporateClientDetailsRepository.Update(inserted);
        var updated = UnitOfWork.CorporateClientDetailsRepository.GetById(id);

        // Assert
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.CompanyName, Is.EqualTo("Updated Company Ltd"));
        Assert.That(updated.ContactPersonName, Is.EqualTo("Jane Smith"));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateDuplicateTaxNumber()
    {
        // Arrange
        var first = CreateValidCorporateClientDetails();
        var second = CreateValidCorporateClientDetails();

        var firstId = UnitOfWork.CorporateClientDetailsRepository.Insert(first);
        var secondId = UnitOfWork.CorporateClientDetailsRepository.Insert(second);

        var firstDetails = UnitOfWork.CorporateClientDetailsRepository.GetById(firstId)!;
        var secondDetails = UnitOfWork.CorporateClientDetailsRepository.GetById(secondId)!;

        secondDetails.TaxNumber = firstDetails.TaxNumber;

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CorporateClientDetailsRepository.Update(secondDetails));
    }

    [Test]
    public void UpdateTest_ShouldNotUpdateNonExistentId()
    {
        // Arrange
        var nonExistent = CreateValidCorporateClientDetails(id: -999);

        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CorporateClientDetailsRepository.Update(nonExistent));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CorporateClientDetailsRepository.Update((CorporateClientDetailsDTO)null!));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var clientDetails = CreateValidCorporateClientDetails();
        var id = UnitOfWork.CorporateClientDetailsRepository.Insert(clientDetails);

        // Act
        UnitOfWork.CorporateClientDetailsRepository.Delete(id);
        var deleted = UnitOfWork.CorporateClientDetailsRepository.GetById(id);

        // Assert
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public void DeleteTest_ShouldNotDeleteInvalidId()
    {
        // Act and Assert
        Assert.Throws<SqlException>(() => UnitOfWork.CorporateClientDetailsRepository.Delete(-999));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CorporateClientDetailsRepository.Delete((object)null!));
    }

    [Test]
    public void GetById_ShouldReturnCorrectCorporateClientDetails()
    {
        // Arrange
        var clientDetails = CreateValidCorporateClientDetails();
        var id = UnitOfWork.CorporateClientDetailsRepository.Insert(clientDetails);

        // Act
        var result = UnitOfWork.CorporateClientDetailsRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }

    [Test]
    public void GetById_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.CorporateClientDetailsRepository.GetById(-1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenDeleted_ShouldReturnNull()
    {
        // Arrange
        var clientDetails = CreateValidCorporateClientDetails();
        var id = UnitOfWork.CorporateClientDetailsRepository.Insert(clientDetails);
        UnitOfWork.CorporateClientDetailsRepository.Delete(id);

        // Act
        var result = UnitOfWork.CorporateClientDetailsRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.CorporateClientDetailsRepository.GetById((object)null!));
    }

    [Test]
    public void GetByTaxNumber_ShouldReturnCorrectCorporateClientDetails()
    {
        // Arrange
        var clientDetails = CreateValidCorporateClientDetails();
        UnitOfWork.CorporateClientDetailsRepository.Insert(clientDetails);

        // Act
        var result = UnitOfWork.CorporateClientDetailsRepository.GetByTaxNumber(clientDetails.TaxNumber);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.TaxNumber, Is.EqualTo(clientDetails.TaxNumber));
    }

    [Test]
    public void GetAll_ShouldExcludeDeletedCorporateClientDetails()
    {
        // Arrange
        var active = CreateValidCorporateClientDetails();
        var deleted = CreateValidCorporateClientDetails();

        var activeId = UnitOfWork.CorporateClientDetailsRepository.Insert(active);
        var deletedId = UnitOfWork.CorporateClientDetailsRepository.Insert(deleted);

        UnitOfWork.CorporateClientDetailsRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.CorporateClientDetailsRepository.GetAll().ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == activeId), Is.True);
        Assert.That(results.Any(c => c.Id == deletedId), Is.False);
    }

    [Test]
    public void GetDeletedCorporateClientDetails_ShouldReturnOnlyDeletedCorporateClientDetails()
    {
        // Arrange
        var active = CreateValidCorporateClientDetails();
        var deleted = CreateValidCorporateClientDetails();

        var activeId = UnitOfWork.CorporateClientDetailsRepository.Insert(active);
        var deletedId = UnitOfWork.CorporateClientDetailsRepository.Insert(deleted);

        UnitOfWork.CorporateClientDetailsRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.CorporateClientDetailsRepository.GetDeletedCorporateClientDetails().ToList();

        // Assert
        Assert.That(results.Any(c => c.Id == deletedId), Is.True);
        Assert.That(results.Any(c => c.Id == activeId), Is.False);
    }
}