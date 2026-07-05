using Market.Repositories;
using Market.Services.Interfaces;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public abstract class BaseRepositoryTests
{
    protected const int UpdateTestId = 1;
    protected const int DeleteTestId = 2;
    protected static string ConnectionString => ConfigurationManager.ConnectionString;
    protected SqlConnection Connection;
    protected IUnitOfWork UnitOfWork;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        DatabaseHelper.ClearDatabase();
        DatabaseHelper.SeedDatabase();
    }

    [SetUp]
    public void Setup()
    {
        Connection = new SqlConnection(ConnectionString);
        UnitOfWork = UnitOfWorkFactory.Create(Connection);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        DatabaseHelper.ClearDatabase();
    }

    [TearDown]
    public void TearDown()
    {
        UnitOfWork.Dispose();
        Connection.Dispose();
    }
}