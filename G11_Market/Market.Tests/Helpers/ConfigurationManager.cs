using Microsoft.Extensions.Configuration;

namespace Market.Tests.Helpers;

public static class ConfigurationManager
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: false)
        .Build();

    public static string ConnectionString =>
        GetEnvironmentConnectionString()
        ?? Configuration.GetConnectionString("MarketDb")
        ?? throw new InvalidOperationException("Connection string 'MarketDb' was not found.");

    private static string? GetEnvironmentConnectionString()
    {
        const string variableName = "MARKET_TEST_CONNECTION_STRING";

        var processValue = Environment.GetEnvironmentVariable(variableName);
        if (!string.IsNullOrWhiteSpace(processValue))
        {
            return processValue;
        }

        if (OperatingSystem.IsWindows())
        {
            var userValue = Environment.GetEnvironmentVariable(
                variableName,
                EnvironmentVariableTarget.User);

            if (!string.IsNullOrWhiteSpace(userValue))
            {
                return userValue;
            }
        }

        return null;
    }
}