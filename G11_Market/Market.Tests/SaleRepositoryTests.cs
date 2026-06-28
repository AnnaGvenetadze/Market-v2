using Market.DTO;
using Market.Repositories;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class SaleRepositoryTests : BaseRepositoryTests
{
    private SqlConnection _connection;
    private SaleRepository _saleRepository;
    private EmployeeRepository _employeeRepository;
    private AccountRepository _accountRepository;
    private int _employeeId;
    private int _secondEmployeeId;
    private int _accountId;
    private int _secondAccountId;

    [SetUp]
    public void Setup()
    {
        _connection = new SqlConnection(ConnectionString);
        _saleRepository = new SaleRepository(_connection);
        _employeeRepository = new EmployeeRepository(_connection);
        _accountRepository = new AccountRepository(_connection);
        var account = new AccountDTO
        {
            Username = "User".AddGuid(),
            PasswordHash = "SecureHash123",
            Email = "lukamania".AddGuid() + "@gmail.com",
            FirstName = "FirstSaxeli",
            LastName = "LastSaxeli",
            AccountType = 1,
        };
        _accountId = _accountRepository.Insert(account);
        var employee1 = new EmployeeDTO
        {
            AccountId = _accountId,
            FirstName = "Test1",
            LastName = "Test1",
            EmployeeCode = "Test1".AddGuid(),
            HireDate = DateTime.Today,
            PhoneNumber = Guid.NewGuid().ToString().Substring(0, 20), 
            ContactEmail = "emp1".AddGuid() + "@market.com"
        };
        _employeeId = _employeeRepository.Insert(employee1);
        var account2 = new AccountDTO
        {
            Username = "User2".AddGuid(),
            PasswordHash = "SecureHash123",
            Email = "lukamania".AddGuid() + "@gmail.com", 
            FirstName = "ManagerFirstName",
            LastName = "ManagerLastName",
            AccountType = 1,
        };
        _secondAccountId = _accountRepository.Insert(account2);
        var employee2 = new EmployeeDTO
        {
            AccountId = _secondAccountId,
            FirstName = "Manager",
            LastName = "Boss",
            EmployeeCode = "Test2".AddGuid(),
            HireDate = DateTime.Today,
            PhoneNumber = Guid.NewGuid().ToString().Substring(0, 20),
            ContactEmail = "emp1".AddGuid() + "@market.com"
        };
        _secondEmployeeId = _employeeRepository.Insert(employee2);
    }

    [Test]
    public void InsertTest_ShouldInsertValidData()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = _employeeId,
            Status = 0,
            CreatedDate = DateTime.Now
        };

        // Act
        var insertedId = _saleRepository.Insert(sale);
        var insertedSale = _saleRepository.GetById(insertedId);

        // Assert
        Assert.That(insertedSale, Is.Not.Null);
        Assert.That(insertedSale.Id, Is.EqualTo(insertedId));
        Assert.That(insertedSale.CreatedEmployeeId, Is.EqualTo(_employeeId));
        Assert.That(insertedSale.Status, Is.EqualTo(0));
    }

    [Test]
    public void UpdateTest_ShouldUpdateValidData()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = _employeeId,
            Status = 0 // Draft
        };
        var insertedId = _saleRepository.Insert(sale);

        var saleToUpdate = _saleRepository.GetById(insertedId);
        saleToUpdate.Status = 1; 

        // Act
        _saleRepository.Update(saleToUpdate);
        var updatedSale = _saleRepository.GetById(insertedId);

        // Assert
        Assert.That(updatedSale, Is.Not.Null);
        Assert.That(updatedSale.Status, Is.EqualTo(1));
    }

    [Test]
    public void DeleteTest_ShouldDeleteValidData()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = _employeeId,
            Status = 0,
        };
        var insertedId = _saleRepository.Insert(sale);

        // Act
        _saleRepository.Delete(insertedId);

        // Assert
        Assert.That(_saleRepository.Search(s => s.Id == insertedId).Any(), Is.False);
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidEmployeeId()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = -9, 
            Status = 0
        };

        // Act & Assert
        Assert.Throws<SqlException>(() => _saleRepository.Insert(sale));
    }

    [Test]
    public void InsertTest_ShouldNotInsertInvalidStatus()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = _employeeId,
            Status = 3 
        };

        // Act & Assert
        Assert.Throws<SqlException>(() => _saleRepository.Insert(sale));
    }

    [Test]
    public void InsertTest_ShouldNotInsertEmptyCancelReason()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = _employeeId,
            Status = 2, // Cancelled
            CancelledByEmployeeId = _secondEmployeeId,
            CancelledDate = DateTime.Now,
            CancelReason = "     "
        };

        // Act & Assert
        Assert.Throws<SqlException>(() => _saleRepository.Insert(sale));
    }

    [Test]
    public void InsertTest_ShouldAllowNullCancelReason()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = _employeeId,
            Status = 0,
            CancelReason = null 
        };

        // Act
        var insertedId = _saleRepository.Insert(sale);
        var insertedSale = _saleRepository.GetById(insertedId);

        // Assert
        Assert.That(insertedSale.CancelReason, Is.Null);
    }

    [Test]
    public void UpdateTest_ShouldAllowValidCancellation()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = _employeeId,
            Status = 0
        };
        var insertedId = _saleRepository.Insert(sale);

        var saleToCancel = _saleRepository.GetById(insertedId);
        saleToCancel.Status = 2; 
        saleToCancel.CancelledByEmployeeId = _secondEmployeeId;
        saleToCancel.CancelledDate = DateTime.Now;
        saleToCancel.CancelReason = "Customer dont want it";

        // Act
        _saleRepository.Update(saleToCancel);
        var updatedSale = _saleRepository.GetById(insertedId);

        // Assert
        Assert.That(updatedSale.Status, Is.EqualTo(2));
        Assert.That(updatedSale.CancelledByEmployeeId, Is.EqualTo(_secondEmployeeId));
        Assert.That(updatedSale.CancelReason, Is.EqualTo("Customer dont want it"));
    }

    [Test]
    public void GetSalesByEmployee_ShouldReturnCorrectData()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = _employeeId,
            Status = 0
        };
        _saleRepository.Insert(sale);

        // Act
        var employeeSales = _saleRepository.GetSalesByEmployee(_employeeId).ToList();

        // Assert
        Assert.That(employeeSales.Any(x => x.CreatedEmployeeId == _employeeId), Is.True);
    }

    [Test]
    public void GetCompletedSales_ShouldReturnCorrectData()
    {
        // Arrange
        var sale = new SaleDTO
        {
            CreatedEmployeeId = _employeeId,
            Status = 1 // 1 = Completed
        };
        var insertedId = _saleRepository.Insert(sale);

        // Act
        var completedSales = _saleRepository.GetCompletedSales().ToList();

        // Assert
        Assert.That(completedSales.Any(x => x.Id == insertedId && x.Status == 1), Is.True);
    }

    [Test]
    public void GetById_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _saleRepository.GetById(null!));
    }

    [Test]
    public void Update_WhenEntityIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _saleRepository.Update(null!));
    }

    [Test]
    public void Delete_WhenIdIsNull_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _saleRepository.Delete(null!));
    }

    [Test]
    public void GetAll_ShouldReturnAllInsertedRecords()
    {
        // Arrange
        var sale1 = new SaleDTO { CreatedEmployeeId = _employeeId, Status = 0 };
        var sale2 = new SaleDTO { CreatedEmployeeId = _employeeId, Status = 1 };

        _saleRepository.Insert(sale1);
        _saleRepository.Insert(sale2);

        // Act
        var allSales = _saleRepository.GetAll().ToList();

        // Assert
        Assert.That(allSales.Count, Is.AtLeast(2));
    }

    [TearDown]
    public void TearDown()
    {
        _saleRepository.Dispose();
        _employeeRepository.Dispose();
        _accountRepository.Dispose();
        _connection.Dispose();
    }
}