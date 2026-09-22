using Market.DTO;
using Market.Services.Interfaces.Repositories;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class EmployeeRepositoryTests : BaseRepositoryTests
{
    private SqlConnection _connection;
    private IEmployeeRepository _employeeRepository;
    private IAccountRepository _accountRepository;
    private int _accountId;

    [SetUp]
    public void Setup()
    {
        _connection = new SqlConnection(ConnectionString);
        _employeeRepository = UnitOfWork.EmployeeRepository;
        _accountRepository = UnitOfWork.AccountRepository;
        var account = new AccountDTO
        {
            Username = "User".AddGuid(),
            PasswordHash = "Password123",
            Email = "Email".AddGuid() + "@gmail.com",
            AccountType = 1,
            FirstName = "Luka",
            LastName = "Mania"
        };
        _accountId = UnitOfWork.AccountRepository.AssignAttribute(account);
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
        var insertedId = UnitOfWork.EmployeeRepository.AssignAttribute(employee);
        var insertedEmployee = UnitOfWork.EmployeeRepository.GetById(insertedId);

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
        var employee = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "Luka",
            lastName: "Mania",
            employeeCode: "EMP2".AddGuid());

        var insertedId = UnitOfWork.EmployeeRepository.AssignAttribute(employee);

        var employeeToUpdate = UnitOfWork.EmployeeRepository.GetById(insertedId);
        employeeToUpdate!.LastName = "NotMania";
        employeeToUpdate.UpdateDate = DateTime.Now;

        // Act
        UnitOfWork.EmployeeRepository.Update(employeeToUpdate);

        var updatedEmployee = UnitOfWork.EmployeeRepository.GetById(insertedId);

        // Assert
        Assert.That(updatedEmployee, Is.Not.Null);
        Assert.That(updatedEmployee.LastName, Is.EqualTo("NotMania"));
    }


    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var employee = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "Deleted",
            lastName: "Account",
            employeeCode: "EMP3".AddGuid());

        var insertedId = UnitOfWork.EmployeeRepository.AssignAttribute(employee);

        // Act
        UnitOfWork.EmployeeRepository.Delete(insertedId);

        // Assert
        Assert.Throws<SqlException>(() => UnitOfWork.EmployeeRepository.GetById(insertedId));
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
        Assert.Throws<SqlException>(() => UnitOfWork.EmployeeRepository.AssignAttribute(employee));
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
        Assert.Throws<SqlException>(() => UnitOfWork.EmployeeRepository.AssignAttribute(employee));
    }


    [Test]
    public void InsertTest_ShouldNotInsertDuplicateEmployeeCode()
    {
        // Arrange
        var sharedCode = "samecode".AddGuid();

        var emp1 = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            employeeCode: sharedCode);

        UnitOfWork.EmployeeRepository.AssignAttribute(emp1);

        var emp2 = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            employeeCode: sharedCode);

        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.EmployeeRepository.AssignAttribute(emp2));
    }


    [Test]
    public void InsertTest_ShouldNotInsertDuplicateContactEmail()
    {
        // Arrange
        var sharedEmail = "shared".AddGuid() + "@gmail.com";

        var emp1 = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            contactEmail: sharedEmail);

        UnitOfWork.EmployeeRepository.AssignAttribute(emp1);

        var emp2 = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            contactEmail: sharedEmail);

        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.EmployeeRepository.AssignAttribute(emp2));
    }


    [Test]
    public void InsertTest_ShouldNotInsertDuplicatePhoneNumber()
    {
        // Arrange
        var sharedPhone = "568".AddGuid();

        var emp1 = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            phoneNumber: sharedPhone);

        UnitOfWork.EmployeeRepository.AssignAttribute(emp1);

        var emp2 = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            phoneNumber: sharedPhone);

        // Act & Assert
        Assert.Throws<SqlException>(() => UnitOfWork.EmployeeRepository.AssignAttribute(emp2));
    }


    [Test]
    public void GetByEmployeeCode_ShouldReturnCorrectEmployee()
    {
        // Arrange
        var targetCode = "code".AddGuid();

        var employee = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "Target",
            lastName: "Employee",
            employeeCode: targetCode);

        UnitOfWork.EmployeeRepository.AssignAttribute(employee);

        // Act
        var result = UnitOfWork.EmployeeRepository.GetByEmployeeCode(targetCode);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.EmployeeCode, Is.EqualTo(targetCode));
    }


    [Test]
    public void GetByEmployeeCode_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.EmployeeRepository.GetByEmployeeCode("QWERTY");

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetByAccountId_ShouldReturnCorrectEmployee()
    {
        // Arrange
        var employee = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "Account",
            lastName: "Owner",
            employeeCode: "EMP6".AddGuid());

        UnitOfWork.EmployeeRepository.AssignAttribute(employee);

        // Act
        var result = UnitOfWork.EmployeeRepository.GetByAccountId(_accountId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.AccountId, Is.EqualTo(_accountId));
    }


    [Test]
    public void GetByAccountId_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = UnitOfWork.EmployeeRepository.GetByAccountId(-99);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void GetSubordinates_ShouldReturnReports()
    {
        // Arrange
        var manager = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "The",
            lastName: "Boss",
            employeeCode: "MGR1".AddGuid());

        var managerId = UnitOfWork.EmployeeRepository.AssignAttribute(manager);

        var sub1 = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "Sub",
            lastName: "One",
            employeeCode: "SUB1".AddGuid(),
            managerEmployeeId: managerId);

        var sub2 = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "Sub",
            lastName: "Two",
            employeeCode: "SUB2".AddGuid(),
            managerEmployeeId: managerId);

        var outsideEmp = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "Other",
            lastName: "Emp",
            employeeCode: "SUB3".AddGuid());

        var id1 = UnitOfWork.EmployeeRepository.AssignAttribute(sub1);
        var id2 = UnitOfWork.EmployeeRepository.AssignAttribute(sub2);
        UnitOfWork.EmployeeRepository.AssignAttribute(outsideEmp);

        // Act
        var subordinates = UnitOfWork.EmployeeRepository.GetSubordinates(managerId).ToList();

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
        var activeEmp = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "Active",
            lastName: "User",
            employeeCode: "ACT1".AddGuid());

        var deletedEmp = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "Deleted",
            lastName: "User",
            employeeCode: "DEL1".AddGuid());

        var activeId = UnitOfWork.EmployeeRepository.AssignAttribute(activeEmp);
        var deletedId = UnitOfWork.EmployeeRepository.AssignAttribute(deletedEmp);

        UnitOfWork.EmployeeRepository.Delete(deletedId);

        // Act
        var activeList = UnitOfWork.EmployeeRepository.GetAllActive().ToList();

        // Assert
        Assert.That(activeList.Any(x => x.Id == activeId), Is.True);
        Assert.That(activeList.Any(x => x.Id == deletedId), Is.False);
        Assert.That(activeList.All(x => !x.IsDeleted), Is.True);
    }


    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.EmployeeRepository.GetById(null!));
    }


    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.EmployeeRepository.Update(null!));
    }


    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UnitOfWork.EmployeeRepository.Delete(null!));
    }


    [Test]
    public void GetAll_ShouldReturnAllInsertedRecords()
    {
        // Arrange
        var emp1 = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "All1",
            lastName: "User",
            employeeCode: "ALL1".AddGuid());

        var emp2 = EmployeeTestDataFactory.CreateEmployee(
            accountId: _accountId,
            firstName: "All2",
            lastName: "User",
            employeeCode: "ALL2".AddGuid());

        UnitOfWork.EmployeeRepository.AssignAttribute(emp1);
        UnitOfWork.EmployeeRepository.AssignAttribute(emp2);

        // Act
        var allEmployees = UnitOfWork.EmployeeRepository.GetAll().ToList();

        // Assert
        Assert.That(allEmployees.Count, Is.AtLeast(2));
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Dispose();
    }
}
