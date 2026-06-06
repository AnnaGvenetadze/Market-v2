using Market.Tests.Helpers;

namespace Market.Tests;

public abstract class BaseRepositoryTests
{
    protected const int UpdateTestId = 1;
    protected const int DeleteTestId = 2;
    protected const string ConnectionString
        //= "Server=.;Database=G11_Market_TEST;Integrated Security=True;TrustServerCertificate=True;";
        = "Server =.; Database=MarketDB_Test;Integrated Security = True; TrustServerCertificate=True;";
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