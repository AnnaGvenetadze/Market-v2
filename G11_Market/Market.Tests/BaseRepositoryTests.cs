using Market.Repositories;
using Market.Services.Interfaces;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public abstract class BaseRepositoryTests
{
    protected const int UpdateTestId = 1;
    protected const int DeleteTestId = 2;
    protected static string ConnectionString 
        => ConfigurationManager.ConnectionString;

    protected SqlConnection Connection;
    protected IUnitOfWork UnitOfWork;

    [SetUp]
    public void BaseSetup()
    {
        DatabaseHelper.ClearDatabase();
        DatabaseHelper.SeedDatabase();

        Connection = new SqlConnection(ConnectionString);
        UnitOfWork = UnitOfWorkFactory.Create(Connection);
    }

    [TearDown]
    public void BaseTearDown()
    {
        UnitOfWork.Dispose();
        Connection.Dispose();
    }
}