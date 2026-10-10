using Market.DTO;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Market.Tests;

[TestFixture]
public class EmployeeRepositoryTests : BaseRepositoryTests
{
    private int CreateAccount(bool isActive = true)
    {
        var account = new AccountDTO
        {
            Username = "User".AddGuid(),
            PasswordHash = "HashedPassword123!",
            IsActive = isActive
        };

        return UnitOfWork.AccountRepository.Insert(account);
    }


    private static EmployeeDTO CreateValidEmployee(
        int accountId,
        int? managerEmployeeId = null) => new()
        {
            AccountId = accountId,
            ManagerEmployeeId = managerEmployeeId,
            FirstName = "John",
            LastName = "Doe",
            EmployeeCode = "EMP".AddGuid(),
            Email = $"user_{Guid.NewGuid():N}@test.com",
            PhoneNumber = TestDataHelper.CreatePhoneNumber(),
            HireDate = DateTime.UtcNow.Date
        };


    private int CreateEmployee(int? managerEmployeeId = null)
    {
        var accountId = CreateAccount();
        var employee = CreateValidEmployee(accountId, managerEmployeeId);

        return UnitOfWork.EmployeeRepository.Insert(employee);
    }


    [Test]
    public void Insert_ShouldInsertValidEmployee()
    {
        // Arrange
        var accountId = CreateAccount();
        var employee = CreateValidEmployee(accountId);

        // Act
        var id = UnitOfWork.EmployeeRepository.Insert(employee);
        var result = UnitOfWork.EmployeeRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(id, Is.GreaterThan(0));
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id));
            Assert.That(result.AccountId, Is.EqualTo(accountId));
            Assert.That(result.FirstName, Is.EqualTo(employee.FirstName));
            Assert.That(result.LastName, Is.EqualTo(employee.LastName));
            Assert.That(result.EmployeeCode, Is.EqualTo(employee.EmployeeCode));
            Assert.That(result.Email, Is.EqualTo(employee.Email));
            Assert.That(result.PhoneNumber, Is.EqualTo(employee.PhoneNumber));
            Assert.That(result.IsDeleted, Is.False);
            Assert.That(result.CreateDate, Is.Not.EqualTo(default(DateTime)));
        });
    }


    [Test]
    public void Insert_WithValidManager_ShouldAssignManager()
    {
        // Arrange
        var managerId = CreateEmployee();
        var accountId = CreateAccount();

        var employee = CreateValidEmployee(
            accountId,
            managerId);

        // Act
        var id = UnitOfWork.EmployeeRepository.Insert(employee);
        var result = UnitOfWork.EmployeeRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ManagerEmployeeId, Is.EqualTo(managerId));
    }


    [Test]
    public void Insert_WhenAccountDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var employee = CreateValidEmployee(int.MaxValue);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(employee));
    }


    [Test]
    public void Insert_WhenAccountIsInactive_ShouldThrowSqlException()
    {
        // Arrange
        var accountId = CreateAccount(false);
        var employee = CreateValidEmployee(accountId);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(employee));
    }


    [Test]
    public void Insert_WhenAccountIsDeleted_ShouldThrowSqlException()
    {
        // Arrange
        var accountId = CreateAccount();
        var employee = CreateValidEmployee(accountId);

        UnitOfWork.AccountRepository.Delete(accountId);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(employee));
    }


    [Test]
    public void Insert_WhenAccountAlreadyAssigned_ShouldThrowSqlException()
    {
        // Arrange
        var accountId = CreateAccount();

        var first = CreateValidEmployee(accountId);
        var second = CreateValidEmployee(accountId);

        UnitOfWork.EmployeeRepository.Insert(first);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(second));
    }


    [Test]
    public void Insert_WhenManagerDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var accountId = CreateAccount();
        var employee = CreateValidEmployee(accountId, int.MaxValue);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(employee));
    }


    [Test]
    public void Insert_WhenEmployeeCodeAlreadyExists_ShouldThrowSqlException()
    {
        // Arrange
        var first = CreateValidEmployee(CreateAccount());
        var second = CreateValidEmployee(CreateAccount());

        second.EmployeeCode = first.EmployeeCode;

        UnitOfWork.EmployeeRepository.Insert(first);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(second));
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Insert_WhenFirstNameIsInvalid_ShouldThrowSqlException(
        string? firstName)
    {
        // Arrange
        var employee = CreateValidEmployee(CreateAccount());
        employee.FirstName = firstName!;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(employee));
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Insert_WhenLastNameIsInvalid_ShouldThrowSqlException(
        string? lastName)
    {
        // Arrange
        var employee = CreateValidEmployee(CreateAccount());
        employee.LastName = lastName!;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(employee));
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Insert_WhenPhoneNumberIsInvalid_ShouldThrowSqlException(
        string? phoneNumber)
    {
        // Arrange
        var employee = CreateValidEmployee(CreateAccount());
        employee.PhoneNumber = phoneNumber!;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(employee));
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase("invalid-email")]
    public void Insert_WhenEmailIsInvalid_ShouldThrowSqlException(
        string? email)
    {
        // Arrange
        var employee = CreateValidEmployee(CreateAccount());
        employee.Email = email!;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(employee));
    }


    [Test]
    public void Insert_WhenHireDateIsInFuture_ShouldThrowSqlException()
    {
        // Arrange
        var employee = CreateValidEmployee(CreateAccount());
        employee.HireDate = DateTime.UtcNow.AddDays(2);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Insert(employee));
    }


    [Test]
    public void Insert_WhenEmployeeIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.EmployeeRepository.Insert(null!));
    }


    [Test]
    public void Update_ShouldUpdateEmployee()
    {
        // Arrange
        var id = CreateEmployee();
        var employee = UnitOfWork.EmployeeRepository.GetById(id)!;

        employee.FirstName = "UpdatedJohn";
        employee.LastName = "UpdatedDoe";
        employee.PhoneNumber = "555123456";
        employee.Email = $"updated_{Guid.NewGuid():N}@test.com";

        // Act
        UnitOfWork.EmployeeRepository.Update(employee);
        var result = UnitOfWork.EmployeeRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FirstName, Is.EqualTo("UpdatedJohn"));
            Assert.That(result.LastName, Is.EqualTo("UpdatedDoe"));
            Assert.That(result.PhoneNumber, Is.EqualTo("555123456"));
            Assert.That(result.Email, Is.EqualTo(employee.Email));
            Assert.That(result.UpdateDate, Is.Not.Null);
        });
    }


    [Test]
    public void Update_WithValidManager_ShouldUpdateManager()
    {
        // Arrange
        var employeeId = CreateEmployee();
        var managerId = CreateEmployee();

        var employee = UnitOfWork.EmployeeRepository
            .GetById(employeeId)!;

        employee.ManagerEmployeeId = managerId;

        // Act
        UnitOfWork.EmployeeRepository.Update(employee);
        var result = UnitOfWork.EmployeeRepository.GetById(employeeId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ManagerEmployeeId, Is.EqualTo(managerId));
    }


    [Test]
    public void Update_WhenEmployeeDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var employee = CreateValidEmployee(CreateAccount());
        employee.Id = int.MaxValue;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Update(employee));
    }


    [Test]
    public void Update_WhenEmployeeIsDeleted_ShouldThrowSqlException()
    {
        // Arrange
        var id = CreateEmployee();
        var employee = UnitOfWork.EmployeeRepository.GetById(id)!;

        UnitOfWork.EmployeeRepository.Delete(id);

        employee.FirstName = "UpdatedJohn";

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Update(employee));
    }


    [Test]
    public void Update_WhenEmployeeCodeAlreadyExists_ShouldThrowSqlException()
    {
        // Arrange
        var firstId = CreateEmployee();
        var secondId = CreateEmployee();

        var first = UnitOfWork.EmployeeRepository.GetById(firstId)!;
        var second = UnitOfWork.EmployeeRepository.GetById(secondId)!;

        second.EmployeeCode = first.EmployeeCode;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Update(second));
    }


    [Test]
    public void Update_WhenEmailAlreadyExists_ShouldThrowSqlException()
    {
        // Arrange
        var firstId = CreateEmployee();
        var secondId = CreateEmployee();

        var first = UnitOfWork.EmployeeRepository.GetById(firstId)!;
        var second = UnitOfWork.EmployeeRepository.GetById(secondId)!;

        second.Email = first.Email;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Update(second));
    }


    [Test]
    public void Update_WhenEmployeeIsOwnManager_ShouldThrowSqlException()
    {
        // Arrange
        var id = CreateEmployee();
        var employee = UnitOfWork.EmployeeRepository.GetById(id)!;

        employee.ManagerEmployeeId = id;

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Update(employee));
    }


    [Test]
    public void Update_WhenPhoneNumberContainsInvalidCharacters_ShouldThrowSqlException()
    {
        // Arrange
        var id = CreateEmployee();
        var employee = UnitOfWork.EmployeeRepository.GetById(id)!;

        employee.PhoneNumber = "555ABC123";

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Update(employee));
    }


    [Test]
    public void Update_WhenEmployeeIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.EmployeeRepository.Update(null!));
    }


    [Test]
    public void Delete_ShouldSoftDeleteEmployee()
    {
        // Arrange
        var id = CreateEmployee();

        // Act
        UnitOfWork.EmployeeRepository.Delete(id);

        var result = UnitOfWork.EmployeeRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void Delete_ShouldNotDeleteAssociatedAccount()
    {
        // Arrange
        var id = CreateEmployee();

        var employee = UnitOfWork.EmployeeRepository.GetById(id)!;
        var accountId = employee.AccountId;

        // Act
        UnitOfWork.EmployeeRepository.Delete(id);

        var account = UnitOfWork.AccountRepository.GetById(accountId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(account, Is.Not.Null);
            Assert.That(account!.IsDeleted, Is.False);
        });
    }


    [Test]
    public void Delete_WhenEmployeeDoesNotExist_ShouldThrowSqlException()
    {
        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Delete(int.MaxValue));
    }


    [Test]
    public void Delete_WhenAlreadyDeleted_ShouldThrowSqlException()
    {
        // Arrange
        var id = CreateEmployee();

        UnitOfWork.EmployeeRepository.Delete(id);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Delete(id));
    }


    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.EmployeeRepository.Delete(null!));
    }


    [Test]
    public void Restore_ShouldRestoreDeletedEmployeeAndAccount()
    {
        // Arrange
        var id = CreateEmployee();
        var employee = UnitOfWork.EmployeeRepository.GetById(id)!;
        var accountId = employee.AccountId;

        UnitOfWork.EmployeeRepository.Delete(id);
        UnitOfWork.AccountRepository.Delete(accountId);

        // Act
        UnitOfWork.EmployeeRepository.Restore(id);

        var restoredEmployee = UnitOfWork.EmployeeRepository.GetById(id);
        var restoredAccount = UnitOfWork.AccountRepository.GetById(accountId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(restoredEmployee, Is.Not.Null);
            Assert.That(restoredEmployee!.IsDeleted, Is.False);
            Assert.That(restoredAccount, Is.Not.Null);
            Assert.That(restoredAccount!.IsDeleted, Is.False);
        });
    }


    [Test]
    public void Restore_WhenEmployeeDoesNotExist_ShouldThrowSqlException()
    {
        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.Restore(int.MaxValue));
    }


    [Test]
    public void Restore_WhenIdIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.EmployeeRepository.Restore(null!));
    }


    [Test]
    public void GetById_ShouldReturnCorrectEmployee()
    {
        // Arrange
        var id = CreateEmployee();

        // Act
        var result = UnitOfWork.EmployeeRepository.GetById(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id));
            Assert.That(result.FirstName, Is.EqualTo("John"));
            Assert.That(result.LastName, Is.EqualTo("Doe"));
        });
    }


    [Test]
    public void GetById_WhenEmployeeDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.EmployeeRepository.GetById(int.MaxValue);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetById_WhenEmployeeIsDeleted_ShouldReturnNull()
    {
        // Arrange
        var id = CreateEmployee();

        UnitOfWork.EmployeeRepository.Delete(id);

        // Act
        var result = UnitOfWork.EmployeeRepository.GetById(id);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetAll_ShouldReturnActiveEmployees()
    {
        // Arrange
        var firstId = CreateEmployee();
        var secondId = CreateEmployee();

        UnitOfWork.EmployeeRepository.Delete(secondId);

        // Act
        var results = UnitOfWork.EmployeeRepository.GetAll().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(e => e.Id == firstId), Is.True);
            Assert.That(results.Any(e => e.Id == secondId), Is.False);
            Assert.That(results.All(e => !e.IsDeleted), Is.True);
        });
    }


    [Test]
    public void GetByAccountId_ShouldReturnCorrectEmployee()
    {
        // Arrange
        var accountId = CreateAccount();
        var employee = CreateValidEmployee(accountId);
        var id = UnitOfWork.EmployeeRepository.Insert(employee);

        // Act
        var result = UnitOfWork.EmployeeRepository
            .GetByAccountId(accountId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id));
            Assert.That(result.AccountId, Is.EqualTo(accountId));
        });
    }


    [Test]
    public void GetByAccountId_WhenEmployeeDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var accountId = CreateAccount();

        // Act
        var result = UnitOfWork.EmployeeRepository
            .GetByAccountId(accountId);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetByAccountId_WhenEmployeeIsDeleted_ShouldReturnNull()
    {
        // Arrange
        var accountId = CreateAccount();
        var employee = CreateValidEmployee(accountId);
        var id = UnitOfWork.EmployeeRepository.Insert(employee);

        UnitOfWork.EmployeeRepository.Delete(id);

        // Act
        var result = UnitOfWork.EmployeeRepository
            .GetByAccountId(accountId);

        // Assert
        Assert.That(result, Is.Null);
    }


    [TestCase(0)]
    [TestCase(-1)]
    public void GetByAccountId_WhenIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int accountId)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.EmployeeRepository.GetByAccountId(accountId));
    }


    [Test]
    public void GetSubordinates_ShouldReturnOnlyManagerEmployees()
    {
        // Arrange
        var managerId = CreateEmployee();

        var firstId = CreateEmployee(managerId);
        var secondId = CreateEmployee(managerId);
        var unrelatedId = CreateEmployee();

        // Act
        var results = UnitOfWork.EmployeeRepository
            .GetSubordinates(managerId)
            .ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(e => e.Id == firstId), Is.True);
            Assert.That(results.Any(e => e.Id == secondId), Is.True);
            Assert.That(results.Any(e => e.Id == unrelatedId), Is.False);
            Assert.That(results.All(e => e.ManagerEmployeeId == managerId), Is.True);
        });
    }


    [Test]
    public void GetSubordinates_WhenManagerHasNoEmployees_ShouldReturnEmpty()
    {
        // Arrange
        var managerId = CreateEmployee();

        // Act
        var results = UnitOfWork.EmployeeRepository
            .GetSubordinates(managerId);

        // Assert
        Assert.That(results, Is.Empty);
    }


    [Test]
    public void GetSubordinates_WhenEmployeeIsDeleted_ShouldExcludeDeletedEmployee()
    {
        // Arrange
        var managerId = CreateEmployee();

        var activeId = CreateEmployee(managerId);
        var deletedId = CreateEmployee(managerId);

        UnitOfWork.EmployeeRepository.Delete(deletedId);

        // Act
        var results = UnitOfWork.EmployeeRepository
            .GetSubordinates(managerId)
            .ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(e => e.Id == activeId), Is.True);
            Assert.That(results.Any(e => e.Id == deletedId), Is.False);
        });
    }


    [TestCase(0)]
    [TestCase(-1)]
    public void GetSubordinates_WhenManagerIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int managerEmployeeId)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.EmployeeRepository.GetSubordinates(managerEmployeeId));
    }


    [Test]
    public void GetRoles_ShouldReturnAssignedRoles()
    {
        // Arrange
        var employeeId = CreateEmployee();
        var role = UnitOfWork.RoleRepository.GetByName("Cashier");

        Assert.That(role, Is.Not.Null);

        UnitOfWork.EmployeeRepository.AssignRole(new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = role!.Id
        });

        // Act
        var results = UnitOfWork.EmployeeRepository
            .GetRoles(employeeId)
            .ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Id, Is.EqualTo(role.Id));
            Assert.That(results[0].Name, Is.EqualTo(role.Name));
        });
    }


    [Test]
    public void GetRoles_WhenEmployeeHasNoRoles_ShouldReturnEmpty()
    {
        // Arrange
        var employeeId = CreateEmployee();

        // Act
        var results = UnitOfWork.EmployeeRepository
            .GetRoles(employeeId);

        // Assert
        Assert.That(results, Is.Empty);
    }


    [TestCase(0)]
    [TestCase(-1)]
    public void GetRoles_WhenEmployeeIdIsInvalid_ShouldThrowArgumentOutOfRangeException(
        int employeeId)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.EmployeeRepository.GetRoles(employeeId));
    }


    [Test]
    public void AssignRole_ShouldAssignRoleToEmployee()
    {
        // Arrange
        var employeeId = CreateEmployee();
        var role = UnitOfWork.RoleRepository.GetByName("Administrator");

        Assert.That(role, Is.Not.Null);

        var employeeRole = new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = role!.Id
        };

        // Act
        UnitOfWork.EmployeeRepository.AssignRole(employeeRole);

        var results = UnitOfWork.EmployeeRepository
            .GetRoles(employeeId)
            .ToList();

        // Assert
        Assert.That(results.Any(r => r.Id == role.Id), Is.True);
    }


    [Test]
    public void AssignRole_WhenAlreadyAssigned_ShouldThrowSqlException()
    {
        // Arrange
        var employeeId = CreateEmployee();
        var role = UnitOfWork.RoleRepository.GetByName("Cashier");

        Assert.That(role, Is.Not.Null);

        var employeeRole = new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = role!.Id
        };

        UnitOfWork.EmployeeRepository.AssignRole(employeeRole);

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.AssignRole(employeeRole));
    }


    [Test]
    public void AssignRole_WhenEmployeeDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var role = UnitOfWork.RoleRepository.GetByName("Cashier");

        Assert.That(role, Is.Not.Null);

        var employeeRole = new EmployeeRoleDTO
        {
            EmployeeId = int.MaxValue,
            RoleId = role!.Id
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.AssignRole(employeeRole));
    }


    [Test]
    public void AssignRole_WhenRoleDoesNotExist_ShouldThrowSqlException()
    {
        // Arrange
        var employeeId = CreateEmployee();

        var employeeRole = new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = int.MaxValue
        };

        // Act & Assert
        Assert.Throws<SqlException>(() =>
            UnitOfWork.EmployeeRepository.AssignRole(employeeRole));
    }


    [Test]
    public void AssignRole_WhenEmployeeRoleIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.EmployeeRepository.AssignRole(null!));
    }


    [TestCase(0, 1)]
    [TestCase(-1, 1)]
    [TestCase(1, 0)]
    [TestCase(1, -1)]
    public void AssignRole_WhenIdsAreInvalid_ShouldThrowArgumentOutOfRangeException(
        int employeeId,
        int roleId)
    {
        // Arrange
        var employeeRole = new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = roleId
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.EmployeeRepository.AssignRole(employeeRole));
    }


    [Test]
    public void UnassignRole_ShouldRemoveAssignedRole()
    {
        // Arrange
        var employeeId = CreateEmployee();
        var role = UnitOfWork.RoleRepository.GetByName("Cashier");

        Assert.That(role, Is.Not.Null);

        var employeeRole = new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = role!.Id
        };

        UnitOfWork.EmployeeRepository.AssignRole(employeeRole);

        // Act
        UnitOfWork.EmployeeRepository.UnassignRole(employeeRole);

        var results = UnitOfWork.EmployeeRepository
            .GetRoles(employeeId)
            .ToList();

        // Assert
        Assert.That(results.Any(r => r.Id == role.Id), Is.False);
    }


    [Test]
    public void UnassignRole_ShouldNotRemoveOtherRoles()
    {
        // Arrange
        var employeeId = CreateEmployee();

        var firstRole = UnitOfWork.RoleRepository.GetByName("Cashier");
        var secondRole = UnitOfWork.RoleRepository.GetByName("Administrator");

        Assert.That(firstRole, Is.Not.Null);
        Assert.That(secondRole, Is.Not.Null);

        var firstEmployeeRole = new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = firstRole!.Id
        };

        var secondEmployeeRole = new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = secondRole!.Id
        };

        UnitOfWork.EmployeeRepository.AssignRole(firstEmployeeRole);
        UnitOfWork.EmployeeRepository.AssignRole(secondEmployeeRole);

        // Act
        UnitOfWork.EmployeeRepository.UnassignRole(firstEmployeeRole);

        var results = UnitOfWork.EmployeeRepository
            .GetRoles(employeeId)
            .ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results.Any(r => r.Id == firstRole.Id), Is.False);
            Assert.That(results.Any(r => r.Id == secondRole.Id), Is.True);
            Assert.That(results.Count, Is.EqualTo(1));
        });
    }


    [Test]
    public void UnassignRole_WhenRoleIsNotAssigned_ShouldNotThrow()
    {
        // Arrange
        var employeeId = CreateEmployee();
        var role = UnitOfWork.RoleRepository.GetByName("Cashier");

        Assert.That(role, Is.Not.Null);

        var employeeRole = new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = role!.Id
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
            UnitOfWork.EmployeeRepository.UnassignRole(employeeRole));
    }


    [Test]
    public void UnassignRole_WhenEmployeeRoleIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            UnitOfWork.EmployeeRepository.UnassignRole(null!));
    }


    [TestCase(0, 1)]
    [TestCase(-1, 1)]
    [TestCase(1, 0)]
    [TestCase(1, -1)]
    public void UnassignRole_WhenIdsAreInvalid_ShouldThrowArgumentOutOfRangeException(
        int employeeId,
        int roleId)
    {
        // Arrange
        var employeeRole = new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = roleId
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitOfWork.EmployeeRepository.UnassignRole(employeeRole));
    }
}
