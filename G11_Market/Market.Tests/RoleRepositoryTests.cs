using Dapper;
using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public sealed class RoleRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void Insert_ShouldCreateRoleWithNullDescription()
    {
        // Arrange
        var role = new RoleDTO
        {
            Name = "Inserted Role".AddGuid(),
            Description = null
        };

        // Act
        var insertedId = UnitOfWork.RoleRepository.Insert(role);

        // Assert
        var storedRole = GetRoleDirect(insertedId);

        Assert.Multiple(() =>
        {
            Assert.That(insertedId, Is.GreaterThan(0));
            Assert.That(storedRole.Name, Is.EqualTo(role.Name));
            Assert.That(storedRole.Description, Is.Null);
            Assert.That(storedRole.IsDeleted, Is.False);
        });
    }

    [Test]
    public void Insert_WithDuplicateName_ShouldThrowUniqueViolation()
    {
        // Arrange
        var duplicateName = "Duplicate Role".AddGuid();
        InsertRoleDirect(duplicateName, "Existing description");

        var duplicateRole = new RoleDTO
        {
            Name = duplicateName,
            Description = "Second description"
        };

        // Act
        var exception = Assert.Throws<SqlException>(
            () => UnitOfWork.RoleRepository.Insert(duplicateRole));

        // Assert
        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Number, Is.AnyOf(2601, 2627));
    }

    [Test]
    public void Update_ShouldChangeRoleFields()
    {
        // Arrange
        var roleId = InsertRoleDirect(
            "Role Before Update".AddGuid(),
            "Before");

        var updatedRole = new RoleDTO
        {
            Id = roleId,
            Name = "Role After Update".AddGuid(),
            Description = "After"
        };

        // Act
        UnitOfWork.RoleRepository.Update(updatedRole);

        // Assert
        var storedRole = GetRoleDirect(roleId);

        Assert.Multiple(() =>
        {
            Assert.That(storedRole.Name, Is.EqualTo(updatedRole.Name));
            Assert.That(storedRole.Description, Is.EqualTo(updatedRole.Description));
            Assert.That(storedRole.IsDeleted, Is.False);
            Assert.That(storedRole.UpdateDate, Is.Not.Null);
        });
    }

    [Test]
    public void Delete_ShouldSoftDeleteRole()
    {
        // Arrange
        var roleId = InsertRoleDirect("Role To Delete".AddGuid());

        // Act
        UnitOfWork.RoleRepository.Delete(roleId);

        // Assert
        var storedRole = GetRoleDirect(roleId);

        Assert.Multiple(() =>
        {
            Assert.That(storedRole.Id, Is.EqualTo(roleId));
            Assert.That(storedRole.IsDeleted, Is.True);
            Assert.That(storedRole.UpdateDate, Is.Not.Null);
        });
    }

    [Test]
    public void GetById_ShouldReturnActiveRole()
    {
        // Arrange
        var roleName = "Role By Id".AddGuid();
        var roleId = InsertRoleDirect(roleName, "Role description");

        // Act
        var result = UnitOfWork.RoleRepository.GetById(roleId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(roleId));
            Assert.That(result.Name, Is.EqualTo(roleName));
            Assert.That(result.Description, Is.EqualTo("Role description"));
            Assert.That(result.IsDeleted, Is.False);
        });
    }

    [Test]
    public void GetAll_ShouldReturnOnlyActiveRoles()
    {
        // Arrange
        var activeRoleId = InsertRoleDirect("Active Role".AddGuid());
        var deletedRoleId = InsertRoleDirect(
            "Deleted Role".AddGuid(),
            isDeleted: true);

        // Act
        var results = UnitOfWork.RoleRepository.GetAll().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(role => role.Id == activeRoleId), Is.True);
            Assert.That(results.Any(role => role.Id == deletedRoleId), Is.False);
            Assert.That(results.All(role => role.IsDeleted == false), Is.True);
        });
    }

    [Test]
    public void GetByName_ShouldReturnMatchingActiveRole()
    {
        // Arrange
        var roleName = "Find Role By Name".AddGuid();
        var roleId = InsertRoleDirect(roleName, "Found role");

        // Act
        var result = UnitOfWork.RoleRepository.GetByName(roleName);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(roleId));
            Assert.That(result.Name, Is.EqualTo(roleName));
            Assert.That(result.IsDeleted, Is.False);
        });
    }

    [Test]
    public void GetByName_WhenMissing_ShouldReturnNull()
    {
        // Arrange
        var missingName = "Missing Role".AddGuid();

        // Act
        var result = UnitOfWork.RoleRepository.GetByName(missingName);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetByName_WhenWhitespace_ShouldThrowArgumentException()
    {
        // Arrange
        const string whitespaceName = "   ";

        // Act and Assert
        Assert.Throws<ArgumentException>(
            () => UnitOfWork.RoleRepository.GetByName(whitespaceName));
    }

    [Test]
    public void GetByName_ShouldExcludeDeletedRole()
    {
        // Arrange
        var roleName = "Deleted Named Role".AddGuid();
        InsertRoleDirect(roleName, isDeleted: true);

        // Act
        var result = UnitOfWork.RoleRepository.GetByName(roleName);

        // Assert
        Assert.That(result, Is.Null);
    }

    private int InsertRoleDirect(
        string name,
        string? description = null,
        bool isDeleted = false)
    {
        return Connection.ExecuteScalar<int>(
            """
            INSERT INTO dbo.Roles
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

    private RoleDTO GetRoleDirect(int roleId)
    {
        return Connection.QuerySingle<RoleDTO>(
            """
            SELECT
                Id,
                Name,
                Description,
                IsDeleted,
                CreateDate,
                UpdateDate
            FROM dbo.Roles
            WHERE Id = @Id;
            """,
            new { Id = roleId });
    }

    [Test]
    public void Restore_ShouldRestoreDeletedRole()
    {
        // Arrange
        UnitOfWork.RoleRepository.Delete(DeleteTestId);

        // Act
        UnitOfWork.RoleRepository.Restore(DeleteTestId);

        // Assert
        var role = UnitOfWork.RoleRepository.GetById(DeleteTestId);

        Assert.That(role, Is.Not.Null);
        Assert.That(role!.IsDeleted, Is.False);
    }

    [Test]
    public void Restore_WhenRoleDoesNotExist_ShouldThrow()
    {
        Assert.Throws<SqlException>(() =>
            UnitOfWork.RoleRepository.Restore(int.MaxValue));
    }

    [Test]
    public void Restore_WhenRoleIsNotDeleted_ShouldThrow()
    {
        Assert.Throws<SqlException>(() =>
            UnitOfWork.RoleRepository.Restore(1));
    }
}
