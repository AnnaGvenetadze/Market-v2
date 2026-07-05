using Microsoft.Extensions.Configuration;

namespace Market.Tests.Helpers;

public static class TestConfigurationHelper
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: false)
        .Build();

    public static string ConnectionString =>
        Configuration.GetConnectionString("MarketTestDb")
        ?? throw new InvalidOperationException("Connection string 'MarketTestDb' was not found.");
}