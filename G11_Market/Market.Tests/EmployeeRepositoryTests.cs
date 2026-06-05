using Market.DTO;
using Market.Repositories;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class EmployeeRepositoryTests
{
    private const string ConnectionString = "Server=localhost;Database=MarketDB_Test;Trusted_Connection=True;TrustServerCertificate=True;";
    private SqlConnection _connection;
    private EmployeeRepository _employeeRepository;
    private AccountRepository _accountRepository;
    private int _accountId;

    [SetUp]
    public void Setup()
    {
        _connection = new SqlConnection(ConnectionString);
        _employeeRepository = new EmployeeRepository(_connection);
        _accountRepository = new AccountRepository(_connection);
        var account = new AccountDTO
        {
            Username = "User".AddGuid(),
            PasswordHash = "Password123",
            Email = "Email".AddGuid() + "@gmail.com",
            AccountType = 1,
            FirstName = "Luka",
            LastName = "Mania"
        };
        _accountId = _accountRepository.Insert(account);
    }

    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var employee = new EmployeeDTO
        {
            AccountId = _accountId,
            FirstName = "Luka",
            LastName = "Mania",
            PhoneNumber = "568".AddGuid(),
            ContactEmail = "luka".AddGuid() + "@gmail.com",
            EmployeeCode = "EMP".AddGuid(),
            HireDate = DateTime.Today,
            IsDeleted = false
        };

        // Act
        var insertedId = _employeeRepository.Insert(employee);
        var insertedEmployee = _employeeRepository.GetById(insertedId);

        // Assert
        Assert.That(insertedEmployee, Is.Not.Null);
        Assert.That(insertedEmployee.Id, Is.EqualTo(insertedId));
        Assert.That(insertedEmployee.FirstName, Is.EqualTo("Luka"));
        Assert.That(insertedEmployee.EmployeeCode, Is.EqualTo(employee.EmployeeCode));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var employee = new EmployeeDTO
        {
            AccountId = _accountId,
            FirstName = "Luka",
            LastName = "Mania",
            EmployeeCode = "EMP2".AddGuid(),
            HireDate = DateTime.Today
        };
        var insertedId = _employeeRepository.Insert(employee);
        var employeeToUpdate = _employeeRepository.GetById(insertedId);
        employeeToUpdate.LastName = "Mania";
        employeeToUpdate.UpdateDate = DateTime.Now;

        // Act
        _employeeRepository.Update(employeeToUpdate);
        var updatedEmployee = _employeeRepository.GetById(insertedId);

        // Assert
        Assert.That(updatedEmployee, Is.Not.Null);
        Assert.That(updatedEmployee.LastName, Is.EqualTo("Mania"));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var employee = new EmployeeDTO
        {
            AccountId = _accountId,
            FirstName = "Deleted",
            LastName = "Accunt",
            EmployeeCode = "EMP3".AddGuid(),
            HireDate = DateTime.Today
        };
        var insertedId = _employeeRepository.Insert(employee);

        // Act
        _employeeRepository.Delete(insertedId);

        // Assert
        Assert.Throws<InvalidOperationException>(() => _employeeRepository.GetById(insertedId));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidAccountId()
    {
        // Arrange
        var employee = new EmployeeDTO
        {
            AccountId = -9,
            FirstName = "Wrong",
            LastName = "Account",
            EmployeeCode = "EMP4".AddGuid(),
            HireDate = DateTime.Today
        };

        // Act & Assert
        Assert.Throws<SqlException>(() => _employeeRepository.Insert(employee));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidManagerId()
    {
        // Arrange
        var employee = new EmployeeDTO
        {
            AccountId = _accountId,
            ManagerEmployeeId = -9,
            FirstName = "Wrong",
            LastName = "Manager",
            EmployeeCode = "EMP5".AddGuid(),
            HireDate = DateTime.Today
        };

        // Act & Assert
        Assert.Throws<SqlException>(() => _employeeRepository.Insert(employee));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateEmployeeCode()
    {
        // Arrange
        var sharedCode = "samecode".AddGuid();
        var emp1 = new EmployeeDTO { AccountId = _accountId, FirstName = "A", LastName = "B", EmployeeCode = sharedCode, HireDate = DateTime.Today };
        _employeeRepository.Insert(emp1);
        var emp2 = new EmployeeDTO { AccountId = _accountId, FirstName = "C", LastName = "D", EmployeeCode = sharedCode, HireDate = DateTime.Today };

        // Act & Assert
        Assert.Throws<SqlException>(() => _employeeRepository.Insert(emp2));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicateContactEmail()
    {
        // Arrange
        var sharedEmail = "shared".AddGuid() + "@gmail.com";
        var emp1 = new EmployeeDTO { AccountId = _accountId, FirstName = "A", LastName = "B", EmployeeCode = "E1".AddGuid(), ContactEmail = sharedEmail, HireDate = DateTime.Today };
        _employeeRepository.Insert(emp1);

        var emp2 = new EmployeeDTO { AccountId = _accountId, FirstName = "C", LastName = "D", EmployeeCode = "E2".AddGuid(), ContactEmail = sharedEmail, HireDate = DateTime.Today };

        // Act & Assert
        Assert.Throws<SqlException>(() => _employeeRepository.Insert(emp2));
    }

    [Test]
    public void InsertTest_ShouldNotInsertDuplicatePhoneNumber()
    {
        // Arrange
        var sharedPhone = "568".AddGuid();
        var emp1 = new EmployeeDTO { AccountId = _accountId, FirstName = "A", LastName = "B", EmployeeCode = "E1".AddGuid(), PhoneNumber = sharedPhone, HireDate = DateTime.Today };
        _employeeRepository.Insert(emp1);

        var emp2 = new EmployeeDTO { AccountId = _accountId, FirstName = "C", LastName = "D", EmployeeCode = "E2".AddGuid(), PhoneNumber = sharedPhone, HireDate = DateTime.Today };

        // Act & Assert
        Assert.Throws<SqlException>(() => _employeeRepository.Insert(emp2));
    }

    [Test]
    public void GetByEmployeeCode_ShouldReturnCorrectEmployee()
    {
        // Arrange
        var targetCode = "code".AddGuid();
        var employee = new EmployeeDTO
        {
            AccountId = _accountId,
            FirstName = "Target",
            LastName = "Employee",
            EmployeeCode = targetCode,
            HireDate = DateTime.Today
        };
        _employeeRepository.Insert(employee);

        // Act
        var result = _employeeRepository.GetByEmployeeCode(targetCode);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.EmployeeCode, Is.EqualTo(targetCode));
    }

    [Test]
    public void GetByEmployeeCode_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = _employeeRepository.GetByEmployeeCode("QWERTY");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetByAccountId_ShouldReturnCorrectEmployee()
    {
        // Arrange
        var employee = new EmployeeDTO
        {
            AccountId = _accountId,
            FirstName = "Account",
            LastName = "Owner",
            EmployeeCode = "EMP6".AddGuid(),
            HireDate = DateTime.Today
        };
        _employeeRepository.Insert(employee);

        // Act
        var result = _employeeRepository.GetByAccountId(_accountId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.AccountId, Is.EqualTo(_accountId));
    }

    [Test]
    public void GetByAccountId_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = _employeeRepository.GetByAccountId(-99);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetSubordinates_ShouldReturntReports()
    {
        // Arrange
        var manager = new EmployeeDTO { AccountId = _accountId, FirstName = "The", LastName = "Boss", EmployeeCode = "MGR1".AddGuid(), HireDate = DateTime.Today };
        var managerId = _employeeRepository.Insert(manager);
        var sub1 = new EmployeeDTO { AccountId = _accountId, FirstName = "Sub", LastName = "One", EmployeeCode = "SUB1".AddGuid(), ManagerEmployeeId = managerId, HireDate = DateTime.Today };
        var sub2 = new EmployeeDTO { AccountId = _accountId, FirstName = "Sub", LastName = "Two", EmployeeCode = "SUB2".AddGuid(), ManagerEmployeeId = managerId, HireDate = DateTime.Today };
        var outsideEmp = new EmployeeDTO { AccountId = _accountId, FirstName = "Other", LastName = "Emp", EmployeeCode = "SUB3".AddGuid(), ManagerEmployeeId = null, HireDate = DateTime.Today };
        var id1 = _employeeRepository.Insert(sub1);
        var id2 = _employeeRepository.Insert(sub2);
        _employeeRepository.Insert(outsideEmp);

        // Act
        var subordinates = _employeeRepository.GetSubordinates(managerId).ToList();

        // Assert
        Assert.That(subordinates.Count, Is.EqualTo(2));
        Assert.That(subordinates.Any(x => x.Id == id1), Is.True);
        Assert.That(subordinates.Any(x => x.Id == id2), Is.True);
        Assert.That(subordinates.All(x => x.ManagerEmployeeId == managerId), Is.True);
    }

    [Test]
    public void GetAllActive_ShouldExcludeDeletedEmployees()
    {
        // Arrange
        var activeEmp = new EmployeeDTO { AccountId = _accountId, FirstName = "Active", LastName = "User", EmployeeCode = "ACT1".AddGuid(), IsDeleted = false, HireDate = DateTime.Today };
        var deletedEmp = new EmployeeDTO { AccountId = _accountId, FirstName = "Deleted", LastName = "User", EmployeeCode = "DEL1".AddGuid(), IsDeleted = true, HireDate = DateTime.Today };

        var activeId = _employeeRepository.Insert(activeEmp);
        var deletedId = _employeeRepository.Insert(deletedEmp);

        // Act
        var activeList = _employeeRepository.GetAllActive().ToList();

        // Assert
        Assert.That(activeList.Any(x => x.Id == activeId), Is.True);
        Assert.That(activeList.Any(x => x.Id == deletedId), Is.False);
        Assert.That(activeList.All(x => !x.IsDeleted), Is.True);
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _employeeRepository.GetById(null!));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _employeeRepository.Update(null!));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _employeeRepository.Delete(null!));
    }

    [Test]
    public void GetAll_ShouldReturnAllInsertedRecords()
    {
        // Arrange
        var emp1 = new EmployeeDTO { AccountId = _accountId, FirstName = "All1", LastName = "User", EmployeeCode = "ALL1".AddGuid(), HireDate = DateTime.Today };
        var emp2 = new EmployeeDTO { AccountId = _accountId, FirstName = "All2", LastName = "User", EmployeeCode = "ALL2".AddGuid(), HireDate = DateTime.Today };

        _employeeRepository.Insert(emp1);
        _employeeRepository.Insert(emp2);

        // Act
        var allEmployees = _employeeRepository.GetAll().ToList();

        // Assert
        Assert.That(allEmployees.Count, Is.AtLeast(2));
    }

    [TearDown]
    public void TearDown()
    {
        _employeeRepository.Dispose();
        _accountRepository.Dispose();
        _connection.Dispose();
    }
}
