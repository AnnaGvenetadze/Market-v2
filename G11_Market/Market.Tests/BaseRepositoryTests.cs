using Market.Repositories;
using Market.Services.Interfaces;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public abstract class BaseRepositoryTests
{
    protected const int UpdateTestId = 1;
    protected const int DeleteTestId = 2;
    //protected const string ConnectionString = "Server=localhost;Database=MarketDB_Test;Trusted_Connection=True;TrustServerCertificate=True;";
    // = "Server=.;Database=G11_Market_Test;Trusted_Connection=True;TrustServerCertificate=True;";
    protected static string ConnectionString => TestConfigurationHelper.ConnectionString;
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