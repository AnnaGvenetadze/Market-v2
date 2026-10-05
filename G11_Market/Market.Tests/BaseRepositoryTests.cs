using Market.Repositories;
using Market.Services.Interfaces;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Market.Tests;

public abstract class BaseRepositoryTests
{
    protected const int UpdateTestId = 1;
    protected const int DeleteTestId = 2;

    protected static string ConnectionString
        => ConfigurationManager.ConnectionString;

    protected SqlConnection Connection = null!;
    protected IUnitOfWork UnitOfWork = null!;

    [SetUp]
    public void BaseSetup()
    {

        Connection = new SqlConnection(ConnectionString);

        UnitOfWork = UnitOfWorkFactory.Create(Connection);
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        DatabaseHelper.ClearDatabase();
        DatabaseHelper.SeedDatabase();
    }

    [TearDown]
    public void BaseTearDown()
    {
        if (UnitOfWork is IDisposable disposableUnitOfWork)
        {
            disposableUnitOfWork.Dispose();
        }

        Connection?.Dispose();
    }
}