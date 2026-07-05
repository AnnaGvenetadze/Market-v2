using Microsoft.Extensions.Configuration;

namespace Market.Tests.Helpers;

public static class ConfigurationManager
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: false)
        .Build();

    public static string ConnectionString =>
        Configuration.GetConnectionString("MarketDb")
        ?? throw new InvalidOperationException("Connection string 'MarketDb' was not found.");
}