using Dapper;
using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public sealed class ClientTypeRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void Insert_ShouldCreateClientTypeWithNullDescription()
    {
        // Arrange
        var clientType = new ClientTypeDTO
        {
            Name = "Inserted Client Type".AddGuid(),
            Description = null
        };

        // Act
        var insertedId = UnitOfWork.ClientTypeRepository.AssignAttribute(clientType);

        // Assert
        var storedClientType = GetClientTypeDirect(insertedId);

        Assert.Multiple(() =>
        {
            Assert.That(insertedId, Is.GreaterThan(0));
            Assert.That(storedClientType.Name, Is.EqualTo(clientType.Name));
            Assert.That(storedClientType.Description, Is.Null);
            Assert.That(storedClientType.IsDeleted, Is.False);
        });
    }

    [Test]
    public void Insert_WithDuplicateName_ShouldThrowUniqueViolation()
    {
        // Arrange
        var duplicateName = "Duplicate Client Type".AddGuid();
        InsertClientTypeDirect(duplicateName, "Existing description");

        var duplicateClientType = new ClientTypeDTO
        {
            Name = duplicateName,
            Description = "Second description"
        };

        // Act
        var exception = Assert.Throws<SqlException>(
            () => UnitOfWork.ClientTypeRepository.AssignAttribute(duplicateClientType));

        // Assert
        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Number, Is.AnyOf(2601, 2627));
    }

    [Test]
    public void Update_ShouldChangeClientTypeFields()
    {
        // Arrange
        var clientTypeId = InsertClientTypeDirect(
            "Client Type Before Update".AddGuid(),
            "Before");

        var updatedClientType = new ClientTypeDTO
        {
            Id = clientTypeId,
            Name = "Client Type After Update".AddGuid(),
            Description = "After"
        };

        // Act
        UnitOfWork.ClientTypeRepository.Update(updatedClientType);

        // Assert
        var storedClientType = GetClientTypeDirect(clientTypeId);

        Assert.Multiple(() =>
        {
            Assert.That(storedClientType.Name, Is.EqualTo(updatedClientType.Name));
            Assert.That(storedClientType.Description, Is.EqualTo(updatedClientType.Description));
            Assert.That(storedClientType.IsDeleted, Is.False);
            Assert.That(storedClientType.UpdateDate, Is.Not.Null);
        });
    }

    [Test]
    public void Delete_ShouldSoftDeleteClientType()
    {
        // Arrange
        var clientTypeId = InsertClientTypeDirect("Client Type To Delete".AddGuid());

        // Act
        UnitOfWork.ClientTypeRepository.Delete(clientTypeId);

        // Assert
        var storedClientType = GetClientTypeDirect(clientTypeId);

        Assert.Multiple(() =>
        {
            Assert.That(storedClientType.Id, Is.EqualTo(clientTypeId));
            Assert.That(storedClientType.IsDeleted, Is.True);
            Assert.That(storedClientType.UpdateDate, Is.Not.Null);
        });
    }

    [Test]
    public void GetById_ShouldReturnActiveClientType()
    {
        // Arrange
        var clientTypeName = "Client Type By Id".AddGuid();
        var clientTypeId = InsertClientTypeDirect(
            clientTypeName,
            "Client type description");

        // Act
        var result = UnitOfWork.ClientTypeRepository.GetById(clientTypeId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(clientTypeId));
            Assert.That(result.Name, Is.EqualTo(clientTypeName));
            Assert.That(result.Description, Is.EqualTo("Client type description"));
            Assert.That(result.IsDeleted, Is.False);
        });
    }

    [Test]
    public void GetAll_ShouldReturnOnlyActiveClientTypes()
    {
        // Arrange
        var activeClientTypeId = InsertClientTypeDirect(
            "Active Client Type".AddGuid());

        var deletedClientTypeId = InsertClientTypeDirect(
            "Deleted Client Type".AddGuid(),
            isDeleted: true);

        // Act
        var results = UnitOfWork.ClientTypeRepository.GetAll().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(
                results.Any(clientType => clientType.Id == activeClientTypeId),
                Is.True);

            Assert.That(
                results.Any(clientType => clientType.Id == deletedClientTypeId),
                Is.False);

            Assert.That(
                results.All(clientType => clientType.IsDeleted == false),
                Is.True);
        });
    }

    private int InsertClientTypeDirect(
        string name,
        string? description = null,
        bool isDeleted = false)
    {
        return Connection.ExecuteScalar<int>(
            """
            INSERT INTO dbo.ClientTypes
            (
                Name,
                Description,
                IsDeleted
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @Name,
                @Description,
                @IsDeleted
            );
            """,
            new
            {
                Name = name,
                Description = description,
                IsDeleted = isDeleted
            });
    }

    private ClientTypeDTO GetClientTypeDirect(int clientTypeId)
    {
        return Connection.QuerySingle<ClientTypeDTO>(
            """
            SELECT
                Id,
                Name,
                Description,
                IsDeleted,
                CreateDate,
                UpdateDate
            FROM dbo.ClientTypes
            WHERE Id = @Id;
            """,
            new { Id = clientTypeId });
    }
}
