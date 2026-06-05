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
    public void Test1()
    {
        Assert.Pass();
    }

    [TearDown]
    public void TearDown()
    {
        _employeeRepository.Dispose();
        _accountRepository.Dispose();
        _connection.Dispose();
    }
}
