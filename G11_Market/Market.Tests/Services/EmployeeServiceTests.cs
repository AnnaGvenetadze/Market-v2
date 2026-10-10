using Market.DTO;
using Market.Services;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using Serilog;

namespace Market.Tests;

[TestFixture]
public class EmployeeServiceTests : BaseRepositoryTests
{
    private EmployeeService _employeeService = null!;
    private const int DefaultRoleId = 1;

    [SetUp]
    public void SetUp()
    {
        var logger = new LoggerConfiguration().CreateLogger();
        _employeeService = new EmployeeService(UnitOfWork, logger);
    }

    private static CreateEmployeeDTO CreateValidEmployeeDto(int roleId = DefaultRoleId) => new()
    {
        FirstName = "First_".AddGuid(),
        LastName = "Last_".AddGuid(),
        Username = $"user_{Guid.NewGuid()}@market.com",
        Password = "Password123!",
        PhoneNumber = $"555-{Random.Shared.Next(1000, 9999)}",
        RoleId = roleId
    };

    [Test]
    public void Constructor_NullUnitOfWork_ShouldThrowArgumentNullException()
    {
        var logger = new LoggerConfiguration().CreateLogger();
        Assert.Throws<ArgumentNullException>(() => new EmployeeService(null!, logger));
    }

    [Test]
    public void Constructor_NullLogger_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new EmployeeService(UnitOfWork, null!));
    }

    [Test]
    public void CreateEmployee_ValidDto_ShouldCreateEmployeeAndAccountAndAssignRole()
    {
        var dto = CreateValidEmployeeDto();

        var created = _employeeService.CreateEmployee(dto);

        Assert.That(created, Is.Not.Null);
        Assert.That(created.Id, Is.GreaterThan(0));
        Assert.That(created.FirstName, Is.EqualTo(dto.FirstName));
        Assert.That(created.LastName, Is.EqualTo(dto.LastName));
        Assert.That(created.AccountId, Is.GreaterThan(0));

        var roles = UnitOfWork.EmployeeRepository.GetRoles(created.Id).ToList();
        Assert.That(roles.Any(r => r.Id == dto.RoleId), Is.True);
    }

    [Test]
    public void CreateEmployee_NullDto_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _employeeService.CreateEmployee(null!));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void CreateEmployee_InvalidFirstName_ShouldThrowArgumentException(string? invalidName)
    {
        var dto = CreateValidEmployeeDto();
        dto.FirstName = invalidName!;

        Assert.Catch<ArgumentException>(() => _employeeService.CreateEmployee(dto));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void CreateEmployee_InvalidLastName_ShouldThrowArgumentException(string? invalidName)
    {
        var dto = CreateValidEmployeeDto();
        dto.LastName = invalidName!;

        Assert.Catch<ArgumentException>(() => _employeeService.CreateEmployee(dto));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void CreateEmployee_InvalidPhoneNumber_ShouldThrowArgumentException(string? invalidPhone)
    {
        var dto = CreateValidEmployeeDto();
        dto.PhoneNumber = invalidPhone!;

        Assert.Catch<ArgumentException>(() => _employeeService.CreateEmployee(dto));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void CreateEmployee_InvalidRoleId_ShouldThrowArgumentOutOfRangeException(int invalidRoleId)
    {
        var dto = CreateValidEmployeeDto(invalidRoleId);

        Assert.Throws<ArgumentOutOfRangeException>(() => _employeeService.CreateEmployee(dto));
    }

    [Test]
    public void GetById_WhenExists_ShouldReturnEmployee()
    {
        var dto = CreateValidEmployeeDto();
        var created = _employeeService.CreateEmployee(dto);

        var result = _employeeService.GetById(created.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(created.Id));
        Assert.That(result.FirstName, Is.EqualTo(dto.FirstName));
    }

    [Test]
    public void GetById_WhenNotFound_ShouldReturnNull()
    {
        var result = _employeeService.GetById(int.MaxValue);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetById_InvalidId_ShouldThrowArgumentOutOfRangeException(int invalidId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _employeeService.GetById(invalidId));
    }

    [Test]
    public void GetAll_ShouldReturnEmployees()
    {
        var dto = CreateValidEmployeeDto();
        var created = _employeeService.CreateEmployee(dto);

        var allEmployees = _employeeService.GetAll().ToList();

        Assert.That(allEmployees.Count, Is.GreaterThan(0));
        Assert.That(allEmployees.Any(e => e.Id == created.Id), Is.True);
    }

    [Test]
    public void ChangeRole_ValidInput_ShouldReassignRole()
    {
        var created = _employeeService.CreateEmployee(CreateValidEmployeeDto(roleId: 1));
        const int newRoleId = 2;

        _employeeService.ChangeRole(created.Id, newRoleId);

        var roles = UnitOfWork.EmployeeRepository.GetRoles(created.Id).ToList();
        Assert.That(roles.Count, Is.EqualTo(1));
        Assert.That(roles.First().Id, Is.EqualTo(newRoleId));
    }

    [Test]
    public void ChangeRole_NonExistingEmployee_ShouldThrowSqlException()
    {
        Assert.Throws<SqlException>(() => _employeeService.ChangeRole(int.MaxValue, 1));
    }

    [TestCase(0, 1)]
    [TestCase(-1, 1)]
    [TestCase(1, 0)]
    [TestCase(1, -1)]
    public void ChangeRole_InvalidParameters_ShouldThrowArgumentOutOfRangeException(int employeeId, int roleId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _employeeService.ChangeRole(employeeId, roleId));
    }

    [Test]
    public void DeactivateEmployee_ValidId_ShouldSoftDeleteEmployeeAndAccount()
    {
        var created = _employeeService.CreateEmployee(CreateValidEmployeeDto());

        _employeeService.DeactivateEmployee(created.Id);

        var employee = _employeeService.GetById(created.Id);
        var account = UnitOfWork.AccountRepository.GetById(created.AccountId);

        Assert.That(employee, Is.Null);
        Assert.That(account, Is.Null);
    }

    [Test]
    public void DeactivateEmployee_NonExistingEmployee_ShouldThrowSqlException()
    {
        Assert.Throws<KeyNotFoundException>(() => _employeeService.DeactivateEmployee(int.MaxValue));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void DeactivateEmployee_InvalidId_ShouldThrowArgumentOutOfRangeException(int invalidId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _employeeService.DeactivateEmployee(invalidId));
    }

    [Test]
    public void ReactivateEmployee_ValidId_ShouldRestoreEmployeeAndAccount()
    {
        var created = _employeeService.CreateEmployee(CreateValidEmployeeDto());
        _employeeService.DeactivateEmployee(created.Id);

        _employeeService.ReactivateEmployee(created.Id);

        var employee = UnitOfWork.EmployeeRepository.GetById(created.Id);
        var account = UnitOfWork.AccountRepository.GetById(created.AccountId);

        Assert.That(employee, Is.Not.Null);
        Assert.That(employee!.IsDeleted, Is.False);
        Assert.That(account, Is.Not.Null);
        Assert.That(account!.IsDeleted, Is.False);
    }

    [Test]
    public void ReactivateEmployee_NonExistingEmployee_ShouldThrowKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => _employeeService.ReactivateEmployee(int.MaxValue));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void ReactivateEmployee_InvalidId_ShouldThrowArgumentOutOfRangeException(int invalidId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _employeeService.ReactivateEmployee(invalidId));
    }

    [Test]
    public void ResetPassword_ValidInput_ShouldUpdateAccountPasswordHash()
    {
        var created = _employeeService.CreateEmployee(CreateValidEmployeeDto());
        const string newPassword = "NewSecretPassword123!";

        _employeeService.ResetPassword(created.Id, newPassword);

        var updatedAccount = UnitOfWork.AccountRepository.GetById(created.AccountId);
        Assert.That(updatedAccount, Is.Not.Null);
        Assert.That(updatedAccount!.PasswordHash, Is.Not.EqualTo("Password123!"));
    }

    [Test]
    public void ResetPassword_NonExistingEmployee_ShouldThrowKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => _employeeService.ResetPassword(int.MaxValue, "NewPass123!"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ResetPassword_NullOrWhitespacePassword_ShouldThrowArgumentException(string? invalidPassword)
    {
        var created = _employeeService.CreateEmployee(CreateValidEmployeeDto());

        Assert.Catch<ArgumentException>(() => _employeeService.ResetPassword(created.Id, invalidPassword!));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void ResetPassword_InvalidEmployeeId_ShouldThrowArgumentOutOfRangeException(int invalidId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _employeeService.ResetPassword(invalidId, "ValidPass123!"));
    }

    [Test]
    public void UpdateEmployee_ValidData_ShouldModifyEmployee()
    {
        var created = _employeeService.CreateEmployee(CreateValidEmployeeDto());
        created.FirstName = "UpdatedFirst_".AddGuid();
        created.LastName = "UpdatedLast_".AddGuid();

        _employeeService.UpdateEmployee(created);

        var updated = _employeeService.GetById(created.Id);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.FirstName, Is.EqualTo(created.FirstName));
        Assert.That(updated.LastName, Is.EqualTo(created.LastName));
    }

    [Test]
    public void UpdateEmployee_NullDto_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _employeeService.UpdateEmployee(null!));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void UpdateEmployee_InvalidId_ShouldThrowArgumentOutOfRangeException(int invalidId)
    {
        var dto = new EmployeeDTO { Id = invalidId, FirstName = "John", LastName = "Doe" };
        Assert.Throws<ArgumentOutOfRangeException>(() => _employeeService.UpdateEmployee(dto));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void UpdateEmployee_InvalidFirstName_ShouldThrowArgumentException(string? invalidName)
    {
        var created = _employeeService.CreateEmployee(CreateValidEmployeeDto());
        created.FirstName = invalidName!;

        Assert.Catch<ArgumentException>(() => _employeeService.UpdateEmployee(created));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void UpdateEmployee_InvalidLastName_ShouldThrowArgumentException(string? invalidName)
    {
        var created = _employeeService.CreateEmployee(CreateValidEmployeeDto());
        created.LastName = invalidName!;

        Assert.Catch<ArgumentException>(() => _employeeService.UpdateEmployee(created));
    }

    [Test]
    public void UpdateEmployee_NonExistingEmployee_ShouldThrowKeyNotFoundException()
    {
        var dto = new EmployeeDTO
        {
            Id = int.MaxValue,
            FirstName = "John",
            LastName = "Doe"
        };

        Assert.Throws<KeyNotFoundException>(() => _employeeService.UpdateEmployee(dto));
    }

    [Test]
    public void UpdateAccount_ValidData_ShouldModifyAccount()
    {
        var created = _employeeService.CreateEmployee(CreateValidEmployeeDto());
        var account = UnitOfWork.AccountRepository.GetById(created.AccountId)!;
        account.Username = $"updated_{Guid.NewGuid()}";

        _employeeService.UpdateAccount(account);

        var updatedAccount = UnitOfWork.AccountRepository.GetById(created.AccountId);
        Assert.That(updatedAccount, Is.Not.Null);
        Assert.That(updatedAccount!.Username, Is.EqualTo(account.Username));
    }

    [Test]
    public void UpdateAccount_NullDto_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _employeeService.UpdateAccount(null!));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void UpdateAccount_InvalidId_ShouldThrowArgumentOutOfRangeException(int invalidId)
    {
        var dto = new AccountDTO { Id = invalidId };
        Assert.Throws<ArgumentOutOfRangeException>(() => _employeeService.UpdateAccount(dto));
    }

    [Test]
    public void UpdateAccount_NonExistingAccount_ShouldThrowKeyNotFoundException()
    {
        var dto = new AccountDTO
        {
            Id = int.MaxValue
        };

        Assert.Throws<KeyNotFoundException>(() => _employeeService.UpdateAccount(dto));
    }
}