using Market.Tests.Helpers;

namespace Market.Tests;

public abstract class BaseRepositoryTests
{
    protected const int UpdateTestId = 1;
    protected const int DeleteTestId = 2;
    protected const string ConnectionString = "Server=localhost;Database=MarketDB_Test;Trusted_Connection=True;TrustServerCertificate=True;";
    // = "Server=.;Database=G11_Market_Test;Trusted_Connection=True;TrustServerCertificate=True;";

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        DatabaseHelper.ClearDatabase();
        DatabaseHelper.SeedDatabase();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        DatabaseHelper.ClearDatabase();
    }
}